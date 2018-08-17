using Lykke.AlgoStore.Job.GDPR.Settings.JobSettings;
using Lykke.AlgoStore.Job.GDPR.Settings.SlackNotifications;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Job.GDPR.Settings
{
    public class AppSettings
    {
        public GdprSettings AlgoStoreGdprJob { get; set; }

        public AlgoStoreStoppingClientSettings AlgoStoreStoppingClient { get; set; }

        public SecurityServiceClientSettings AlgoStoreSecurityServiceClient { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
