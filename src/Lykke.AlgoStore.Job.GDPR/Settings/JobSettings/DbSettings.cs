using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.GDPR.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnectionString { get; set; }

        [AzureTableCheck]
        public string DataStorageConnectionString { get; set; }
    }
}
