using Common.Log;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Settings.JobSettings;
using Lykke.Common.Log;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR
{
    public class DeactivatedSubscribersMonitor
    {
        private readonly DeactivatedSubscribersMonitorSettings _settings;
        private readonly ILog _log;
        private readonly ISubscriberRepository _subscriberRepository;

        public DeactivatedSubscribersMonitor(DeactivatedSubscribersMonitorSettings settings,
            ILogFactory logFactory, ISubscriberRepository subscriberRepository)
        {
            _settings = settings;
            _log = logFactory.CreateLog(this);
            _subscriberRepository = subscriberRepository;
        }

        public async Task StartAsync()
        {
            await ExecuteAsync();
        }

        private async Task ExecuteAsync()
        {
            while (true)
            {
                var deactivatedSubscribers = _subscriberRepository.GetSuscribersToDeactivateAsync();

                //await TryDeleteSubscribersInformation(deactivatedSubscribers);

                await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalInSeconds));
            }
        }
    }
}
