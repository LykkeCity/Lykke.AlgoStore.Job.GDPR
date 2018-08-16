using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Services.Strings;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Security.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly IAlgoCommentsRepository _commentsRepository;
        private readonly ISecurityClient _securityClient;
        private readonly IAlgoInstanceStoppingClient _algoInstanceStoppingClient;
        private readonly IAlgoRepository _algoRepository;


        public UsersService(IUsersRepository usersRepository,
            IAlgoCommentsRepository commentsRepository,
            ISecurityClient securityClient,
            IAlgoInstanceStoppingClient algoInstanceStoppingClient,
            IAlgoClientInstanceRepository instanceRepository,
            IAlgoRepository algoRepository)
        {
            _usersRepository = usersRepository;
            _commentsRepository = commentsRepository;
            _securityClient = securityClient;
            _algoInstanceStoppingClient = algoInstanceStoppingClient;
            _instanceRepository = instanceRepository;
            _algoRepository = algoRepository;
        }

        public async Task SeedAsync(string clientId)
        {
            var entity = await _usersRepository.GetByIdAsync(clientId);

            if (entity == null)
            {
                entity = new UserData
                {
                    ClientId = clientId,
                    CookieConsent = false,
                    GdprConsent = false,
                    DeletionStatus = DeletionStatus.None
                };
            }

            await _usersRepository.UpdateAsync(entity);
        }

        public async Task<UserData> GetByIdAsync(string clientId)
        {
            var result = await _usersRepository.GetByIdAsync(clientId);

            return result;
        }

        public async Task SetCookieConsentAsync(string clientId)
        {
            var entity = await _usersRepository.GetByIdAsync(clientId);

            if (entity == null)
            {
                entity = new UserData
                {
                    ClientId = clientId
                };
            }

            if (entity.CookieConsent)
            {
                throw new ValidationException(string.Format(Phrases.ConsentAlreadyGiven, "Cookie", clientId));
            }

            entity.CookieConsent = true;

            await _usersRepository.UpdateAsync(entity);
        }

        public async Task SetGdprConsentAsync(string clientId)
        {
            var entity = await _usersRepository.GetByIdAsync(clientId);

            if (entity == null)
            {
                entity = new UserData
                {
                    ClientId = clientId
                };
            }

            if (entity.GdprConsent)
            {
                throw new ValidationException(string.Format(Phrases.ConsentAlreadyGiven, "GDPR", clientId));
            }

            entity.GdprConsent = true;

            await _usersRepository.UpdateAsync(entity);
        }

        public async Task RemoveUserConsents(string clientId)
        {
            await _usersRepository.DeleteAsync(clientId);
        }

        public async Task DeactivateAccountAsync(string clientId)
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

            // remove user legal consent
            await RemoveUserConsents(clientId);

            // find and stop all instances
            var instances = await _instanceRepository.GetAllAlgoInstancesByClientAsync(clientId);

            foreach (var instance in instances)
            {
                var result =
                    await _algoInstanceStoppingClient.DeleteAlgoInstanceAsync(instance.InstanceId, instance.AuthToken);
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
        }
    }
}
