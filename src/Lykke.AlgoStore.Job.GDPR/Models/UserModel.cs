namespace Lykke.AlgoStore.Job.GDPR.Models
{
    public class UserModel
    {
        public string ClientId { get; set; }
        public bool GDPRConsent { get; set; }
        public bool CookieConsent { get; set; }
    }
}
