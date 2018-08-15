using Lykke.AlgoStore.Job.GDPR.Settings.JobSettings;
using Lykke.AlgoStore.Job.GDPR.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Job.GDPR.Settings
{
    public class AppSettings
    {
        public GDPRSettings AlgoStoreGDPRJob { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
