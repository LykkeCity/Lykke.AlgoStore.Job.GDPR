using AzureStorage;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.AzureRepositories.Repositories
{
    public class SubscriberRepository : ISubscriberRepository
    {
        public static readonly string TableName = "AlgoStoreSubscribers";
        private static readonly string PartitionKey = "Subscriber";

        private readonly INoSQLTableStorage<SubscriberEntity> _table;

        public static string GenerateRowKey(string key) => key;

        public SubscriberRepository(INoSQLTableStorage<SubscriberEntity> table)
        {
            _table = table;
        }

        public async Task SaveAsync(SubscriberData data)
        {
            var entity = AutoMapper.Mapper.Map<SubscriberEntity>(data);
            entity.PartitionKey = PartitionKey;
            entity.RowKey = GenerateRowKey(data.ClientId);

            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task<SubscriberData> GetByIdAsync(string clientId)
        {
            var result = await _table.GetDataAsync(PartitionKey, clientId);

            return AutoMapper.Mapper.Map<SubscriberData>(result);
        }

        public async Task UpdateAsync(SubscriberData data)
        {
            var entity = AutoMapper.Mapper.Map<SubscriberEntity>(data);
            entity.PartitionKey = PartitionKey;
            entity.RowKey = GenerateRowKey(data.ClientId);

            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task DeleteAsync(string clientId)
        {
            await _table.DeleteIfExistAsync(PartitionKey, clientId);
        }
    }
}
