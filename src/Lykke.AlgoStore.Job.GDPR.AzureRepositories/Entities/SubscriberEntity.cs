using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities
{
    public class SubscriberEntity : TableEntity
    {
        public bool GdprConsent { get; set; }
        public bool CookieConsent { get; set; }
        public DeletionStatus DeletionStatus { get; set; }
    }
}
