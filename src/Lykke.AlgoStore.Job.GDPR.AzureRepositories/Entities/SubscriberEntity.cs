using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AzureStorage.Tables;

namespace Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities
{
    public class SubscriberEntity : AzureTableEntity
    {
        public bool GdprConsent { get; set; }
        public bool CookieConsent { get; set; }
        public DeletionStatus DeletionStatus { get; set; }
    }
}
