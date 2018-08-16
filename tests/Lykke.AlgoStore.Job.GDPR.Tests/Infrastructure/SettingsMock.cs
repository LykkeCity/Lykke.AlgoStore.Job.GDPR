using Lykke.AlgoStore.Job.GDPR.Settings;
using Lykke.AlgoStore.Job.GDPR.Settings.JobSettings;
using Lykke.SettingsReader;
using Moq;

namespace Lykke.AlgoStore.Job.GDPR.Tests.Infrastructure
{
    public class SettingsMock
    {
        public static IReloadingManager<string> GetDataStorageConnectionString()
        {
            var config = InitConfig();

            return config.ConnectionString(x => x.AlgoStoreGdprJob.Db.DataStorageConnectionString);
        }

        private static IReloadingManager<AppSettings> InitConfig()
        {
            var reloadingMock = new Mock<IReloadingManager<AppSettings>>();

            reloadingMock.Setup(x => x.CurrentValue)
                .Returns(new AppSettings
                {
                    AlgoStoreGdprJob = new GdprSettings
                    {
                        Db = new DbSettings
                        {
                            DataStorageConnectionString = "UseDevelopmentStorage=true",
                            LogsConnectionString = "UseDevelopmentStorage=true"
                        }
                    }
                });

            var config = reloadingMock.Object;

            return config;
        }
    }
}
