using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.Core.Services
{
    public interface ISubscriberService
    {
        Task SeedAsync(string clientId);
        Task<SubscriberData> GetByIdAsync(string clientId);
        Task SetGdprConsentAsync(string clientId);
        Task SetCookieConsentAsync(string clientId);
        Task DeactivateAccountAsync(string clientId);
    }
}
