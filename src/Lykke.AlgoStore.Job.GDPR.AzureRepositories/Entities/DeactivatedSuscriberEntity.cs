using Lykke.AzureStorage.Tables;

namespace Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities
{
    public class DeactivatedSuscriberEntity : AzureTableEntity
    {
        public string ClientId { get; set; }
    }
}
