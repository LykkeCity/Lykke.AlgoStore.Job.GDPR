using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories
{
    public interface ISubscriberRepository
    {
        Task<SubscriberData> GetByIdAsync(string clientId);
        Task SaveAsync(SubscriberData data);
        Task UpdateAsync(SubscriberData data);
        Task DeleteAsync(string clientId);
        Task<ICollection<DeactivateSubscriberData>> GetSuscribersToDeactivateAsync();
        Task DeleteDeactivatedSubscriberAsync(string clientId);
    }
}
