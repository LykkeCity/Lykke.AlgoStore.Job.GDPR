using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Services.Strings;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Security.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.AlgoStore.Job.GDPR.Services
{
    public class SubscriberService : ISubscriberService
    {
        private readonly ISubscriberRepository _usersRepository;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly IAlgoCommentsRepository _commentsRepository;
        private readonly ISecurityClient _securityClient;
        private readonly IAlgoInstanceStoppingClient _algoInstanceStoppingClient;
        private readonly IAlgoRepository _algoRepository;
        private readonly ILog _log;


        public SubscriberService(ISubscriberRepository usersRepository,
            IAlgoCommentsRepository commentsRepository,
            ISecurityClient securityClient,
            IAlgoInstanceStoppingClient algoInstanceStoppingClient,
            IAlgoClientInstanceRepository instanceRepository,
            IAlgoRepository algoRepository, ILogFactory log)
        {
            _usersRepository = usersRepository;
            _commentsRepository = commentsRepository;
            _securityClient = securityClient;
            _algoInstanceStoppingClient = algoInstanceStoppingClient;
            _instanceRepository = instanceRepository;
            _algoRepository = algoRepository;
            _log = log.CreateLog(this);
        }

        public async Task SeedAsync(string clientId)
        {
            ValidateClientId(clientId);

            var entity = await _usersRepository.GetByIdAsync(clientId);

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

            await _usersRepository.UpdateAsync(entity);
        }

        public async Task<SubscriberData> GetByIdAsync(string clientId)
        {
            ValidateClientId(clientId);

            var result = await _usersRepository.GetByIdAsync(clientId);

            return result;
        }

        public async Task SetCookieConsentAsync(string clientId)
        {
            ValidateClientId(clientId);

            var entity = await _usersRepository.GetByIdAsync(clientId);

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

            await _usersRepository.UpdateAsync(entity);
        }

        public async Task SetGdprConsentAsync(string clientId)
        {
            ValidateClientId(clientId);

            var subscriberData = await _usersRepository.GetByIdAsync(clientId);

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

            await _usersRepository.UpdateAsync(subscriberData);
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
                // probably set deletion status here? 

                // first get all comments made by the user and unlink his id from author field
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

                // second get all roles for this user

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
                }

                // replace author in algos
                var algos = await _algoRepository.GetAllClientAlgosAsync(clientId);

                foreach (var algo in algos)
                {
                    // update it
                    var algoToSave = AutoMapper.Mapper.Map<IAlgo>(algo);
                    algoToSave.ClientId = Guid.NewGuid().ToString();
                    algoToSave.DateModified = DateTime.Now;
                    await _algoRepository.SaveAlgoWithNewPKAsync(algoToSave, algo.ClientId);
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

            await _usersRepository.DeleteAsync(clientId);
        }

        /// <summary>
        /// Deletes the row which shows the client whose information which should be replaced/deactivated
        /// </summary>
        public async Task DeleteDeactivatedSubscriberRow(string clientId)
        {
            ValidateClientId(clientId);

            await _usersRepository.DeleteDeactivatedSubscriberAsync(clientId);
        }

        /// <summary>
        /// Get all subscribers whose information should be replaced/deactivated.
        /// </summary>
        public async Task<ICollection<DeactivateSubscriberData>> GetSuscribersToDeactivateAsync()
        {
            return await _usersRepository.GetSuscribersToDeactivateAsync();
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
