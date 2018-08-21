using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Job.GDPR.Settings
{
    public class MonitoringServiceClientSettings
    {
        [HttpCheck("api/isalive", false)]
        public string MonitoringServiceUrl { get; set; }
    }
}
