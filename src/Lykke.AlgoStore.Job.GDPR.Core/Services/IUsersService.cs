using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.Core.Services
{
    public interface IUsersService
    {
        Task SeedAsync(string clientId);
        Task<UserData> GetByIdAsync(string clientId);
        Task SetGDPRConsentAsync(string clientId);
        Task SetCookieConsentAsync(string clientId);
        Task DeactivateAccountAsync(string clientId);
    }
}
