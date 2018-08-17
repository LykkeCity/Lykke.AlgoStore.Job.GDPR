using AzureStorage.Tables;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories.Repositories;
using Lykke.Common.Log;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.Job.GDPR.AzureRepositories
{
    public class AzureRepoFactories
    {
        public static SubscriberRepository CreateSubscriberRepository(IReloadingManager<string> connectionString,
            ILogFactory log)
        {
            return new SubscriberRepository(
                AzureTableStorage<SubscriberEntity>.Create(connectionString, SubscriberRepository.TableName, log),
                AzureTableStorage<DeactivatedSuscriberEntity>.Create(connectionString, SubscriberRepository.TableName, log));
        }
    }
}
