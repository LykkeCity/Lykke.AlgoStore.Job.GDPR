using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Services;
using Lykke.AlgoStore.Job.GDPR.Settings;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Job.GDPR.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settingsManager;
        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly IServiceCollection _services;

        public JobModule(IReloadingManager<AppSettings> settingsManager)
        {
            _settingsManager = settingsManager;
            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepositories(builder);
            RegisterDeactivationProcess(builder);
            RegisterLocalServices(builder);
            RegisterExternalServices(builder);

            builder.Populate(_services);
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

            var logFactory = LogFactory.Create().AddConsole();
            var reloadingDbManager =
                _settingsManager.ConnectionString(x => x.AlgoStoreGdprJob.Db.DataStorageConnectionString);

            builder.RegisterInstance(AzureTableStorage<AlgoCommentEntity>.Create(reloadingDbManager,
                AlgoCommentsRepository.TableName, logFactory));
            builder.RegisterType<AlgoCommentsRepository>().As<IAlgoCommentsRepository>();

            builder.Register(x =>
                {
                    var log = x.Resolve<ILogFactory>();
                    var repository = CSharp.AlgoTemplate.Models.AzureRepoFactories
                        .CreateAlgoClientInstanceRepository(
                            _settingsManager.Nested(r => r.AlgoStoreGdprJob.Db.DataStorageConnectionString),
                            log.CreateLog(this));

                    return repository;
                })
                .As<IAlgoClientInstanceRepository>()
                .SingleInstance();

            builder.RegisterInstance(
                AzureTableStorage<AlgoEntity>.Create(reloadingDbManager, AlgoRepository.TableName, logFactory));

            builder.RegisterType<AlgoRepository>().As<IAlgoReadOnlyRepository>().As<IAlgoRepository>();

            builder.RegisterInstance(AzureTableStorage<PublicAlgoEntity>.Create(reloadingDbManager, PublicAlgosRepository.TableName, logFactory.CreateLog(this)));
            builder.RegisterType<PublicAlgosRepository>().As<IPublicAlgosRepository>();
        }

        private void RegisterDeactivationProcess(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settingsManager.CurrentValue.AlgoStoreGdprJob.DeactivatedSubscribersMonitor);
            builder.RegisterType<DeactivatedSubscribersMonitor>().SingleInstance();
        }

        private void RegisterExternalServices(ContainerBuilder builder)
        {
            var logFactory = LogFactory.Create().AddConsole();

            builder.RegisterAlgoInstanceStoppingClient(_settingsManager.CurrentValue.AlgoStoreStoppingClient.ServiceUrl,
                logFactory.CreateLog(this));

            builder.RegisterSecurityClient(_settingsManager.CurrentValue.AlgoStoreSecurityServiceClient.ServiceUrl, logFactory.CreateLog(this));

        }

        private void RegisterLocalServices(ContainerBuilder builder)
        {
            builder.RegisterType<SubscriberService>()
                .As<ISubscriberService>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();
        }
    }
}
