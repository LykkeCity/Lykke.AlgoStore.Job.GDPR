using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Services;
using Lykke.AlgoStore.Job.GDPR.Settings;
using Lykke.Common.Log;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Job.GDPR.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settingsManager;
        private readonly IServiceCollection _services;

        public JobModule(IReloadingManager<AppSettings> settingsManager)
        {
            _settingsManager = settingsManager;
            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepositories(builder);

            RegisterServices(builder);

            RegisterDeactivationProcess(builder);

            builder.Populate(_services);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<SubscriberService>().As<ISubscriberService>().SingleInstance();
        }

        private void RegisterRepositories(ContainerBuilder builder)
        {
            builder.Register(x =>
            {
                var log = x.Resolve<ILogFactory>();
                var repository = AzureRepoFactories.CreateSubscriberRepository(
                    _settingsManager.Nested(r => r.AlgoStoreGdprJob.Db.DataStorageConnectionString), log);

                return repository;
            })
            .As<ISubscriberRepository>()
            .SingleInstance();
        }

        private void RegisterDeactivationProcess(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settingsManager.CurrentValue.AlgoStoreGdprJob.DeactivatedSubscribersMonitor);
            builder.RegisterType<DeactivatedSubscribersMonitor>().SingleInstance();
        }
    }
}
