namespace Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities
{
    public class SubscriberData
    {
        public string ClientId { get; set; }
        public bool GdprConsent { get; set; }
        public bool CookieConsent { get; set; }
        public DeletionStatus DeletionStatus { get; set; }
    }

    public enum DeletionStatus
    {
        None = 0,
        InProgress = 1,
        Done = 2
    }
}
