using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Services.Strings;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.Common.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.Services
{
    public class SubscriberService : ISubscriberService
    {
        private readonly ISubscriberRepository _subscribersRepository;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly IAlgoCommentsRepository _commentsRepository;
        private readonly ISecurityClient _securityClient;
        private readonly IAlgoInstanceStoppingClient _algoInstanceStoppingClient;
        private readonly IAlgoRepository _algoRepository;
        private readonly ILog _log;
        private readonly IPublicAlgosRepository _publicAlgosRepository;

        public SubscriberService(ISubscriberRepository usersRepository,
            IAlgoCommentsRepository commentsRepository,
            ISecurityClient securityClient,
            IAlgoInstanceStoppingClient algoInstanceStoppingClient,
            IAlgoClientInstanceRepository instanceRepository,
            IAlgoRepository algoRepository, ILogFactory log,
            IPublicAlgosRepository publicAlgosRepository
            )
        {
            _subscribersRepository = usersRepository;
            _commentsRepository = commentsRepository;
            _securityClient = securityClient;
            _algoInstanceStoppingClient = algoInstanceStoppingClient;
            _instanceRepository = instanceRepository;
            _algoRepository = algoRepository;
            _log = log.CreateLog(this);
            _publicAlgosRepository = publicAlgosRepository;
        }

        public async Task SeedAsync(string clientId)
        {
            ValidateClientId(clientId);

            var entity = await _subscribersRepository.GetByIdAsync(clientId);

            if (entity == null)
            {
                entity = new SubscriberData
                {
                    ClientId = clientId,
                    CookieConsent = false,
                    GdprConsent = false,
                    DeletionStatus = DeletionStatus.None
                };
            }

            await _subscribersRepository.UpdateAsync(entity);
        }

        public async Task<SubscriberData> GetByIdAsync(string clientId)
        {
            ValidateClientId(clientId);

            var result = await _subscribersRepository.GetByIdAsync(clientId);

            return result;
        }

        public async Task SetCookieConsentAsync(string clientId)
        {
            ValidateClientId(clientId);

            var entity = await _subscribersRepository.GetByIdAsync(clientId);

            if (entity == null)
            {
                entity = new SubscriberData
                {
                    ClientId = clientId
                };
            }

            if (entity.CookieConsent)
            {
                throw new ValidationException(Phrases.CookieConsentAlreadyGiven);
            }

            entity.CookieConsent = true;

            await _subscribersRepository.UpdateAsync(entity);
        }

        public async Task SetGdprConsentAsync(string clientId)
        {
            ValidateClientId(clientId);

            var subscriberData = await _subscribersRepository.GetByIdAsync(clientId);

            if (subscriberData == null)
            {
                subscriberData = new SubscriberData
                {
                    ClientId = clientId
                };
            }

            if (subscriberData.GdprConsent)
            {
                throw new ValidationException(Phrases.GdprConsentAlreadyGiven);
            }

            subscriberData.GdprConsent = true;

            await _subscribersRepository.UpdateAsync(subscriberData);
        }

        /// <summary>
        /// Removes sensitive information for the subscriber and replaces the other information with a fake id.
        /// </summary>
        /// <returns>
        ///Returns information if the deactivation was successful.
        /// </returns>
        public async Task<bool> DeactivateAccountAsync(string clientId)
        {
            bool isSuccessfulDeactivation;

            ValidateClientId(clientId);

            try
            {
                ValidateClientId(clientId);

                //get all comments made by the user and unlink his id from author field
                var comments = await _commentsRepository.GetAllAsync();
                var userComments = comments.FindAll(c => c.Author == clientId);

                foreach (var comment in userComments)
                {
                    if (comment.Author == clientId)
                    {
                        comment.Author = null;
                        await _commentsRepository.SaveCommentAsync(comment);
                    }
                }

                //get all roles for this user
                var user = await _securityClient.GetUserByIdWithRolesAsync(clientId);
                var roles = user.Roles;

                // delete his roles
                foreach (var role in roles)
                {
                    await _securityClient.RevokeRoleFromUserAsync(
                        new Lykke.Service.Security.Client.AutorestClient.Models.UserRoleMatchModel()
                        {
                            ClientId = clientId,
                            RoleId = role.Id
                        });
                }

                // find and stop all instances
                var instances = await _instanceRepository.GetAllAlgoInstancesByClientAsync(clientId);

                foreach (var instance in instances)
                {
                    var result =
                        await _algoInstanceStoppingClient.DeleteAlgoInstanceAsync(instance.InstanceId,
                            instance.AuthToken);
                    await _instanceRepository.SaveAlgoInstanceWithNewPKAsync(instance);
                }

                // replace author in algos
                var algos = await _algoRepository.GetAllClientAlgosAsync(clientId);

                foreach (var algo in algos)
                {
                    // update it
                    var algoToSave = AutoMapper.Mapper.Map<IAlgo>(algo);
                    algoToSave.ClientId = "Deactivated";
                    algoToSave.DateModified = DateTime.Now;
                    await _algoRepository.SaveAlgoWithNewPKAsync(algoToSave, algo.ClientId);

                    if (await _publicAlgosRepository.ExistsPublicAlgoAsync(clientId, algoToSave.AlgoId))
                    {
                        await _publicAlgosRepository.SavePublicAlgoNewPKAsync(new PublicAlgoData()
                        {
                            ClientId = clientId,
                            AlgoId = algoToSave.AlgoId
                        });
                    }
                }

                isSuccessfulDeactivation = true;
            }
            catch (Exception ex)
            {
                _log.Error(exception: ex, message: "There was a problem with account deactivation");

                isSuccessfulDeactivation = false;
            }

            return isSuccessfulDeactivation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task RemoveUserConsents(string clientId)
        {
            ValidateClientId(clientId);

            await _subscribersRepository.DeleteAsync(clientId);
        }

        /// <summary>
        /// Deletes the row which shows the client whose information which should be replaced/deactivated
        /// </summary>
        public async Task DeleteDeactivatedSubscriberRow(string clientId)
        {
            ValidateClientId(clientId);

            await _subscribersRepository.DeleteDeactivatedSubscriberAsync(clientId);
        }

        /// <summary>
        /// Get all subscribers whose information should be replaced/deactivated.
        /// </summary>
        public async Task<ICollection<DeactivateSubscriberData>> GetSuscribersToDeactivateAsync()
        {
            return await _subscribersRepository.GetSuscribersToDeactivateAsync();
        }

        private void ValidateClientId(string clientId)
        {
            if (clientId == null)
                throw new ArgumentNullException(nameof(clientId));

            if (clientId == String.Empty)
                throw new ValidationException(Phrases.ClientIdEmpty);
        }
    }
}
