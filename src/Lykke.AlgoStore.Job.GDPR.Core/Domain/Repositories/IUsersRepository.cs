using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories
{
    public interface IUsersRepository
    {
        Task<UserData> GetByIdAsync(string clientId);
        Task SaveAsync(UserData data);
        Task UpdateAsync(UserData data);
        Task DeleteAsync(string clientId);
    }
}
