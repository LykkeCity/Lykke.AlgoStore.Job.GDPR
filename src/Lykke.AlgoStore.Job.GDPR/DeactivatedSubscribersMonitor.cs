using Common.Log;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Repositories;
using Lykke.AlgoStore.Job.GDPR.Settings.JobSettings;
using Lykke.Common.Log;
using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.Job.GDPR.Core.Services;

namespace Lykke.AlgoStore.Job.GDPR
{
    public class DeactivatedSubscribersMonitor
    {
        private readonly DeactivatedSubscribersMonitorSettings _settings;
        private readonly ILog _log;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriberService _subscriberService;

        public DeactivatedSubscribersMonitor(DeactivatedSubscribersMonitorSettings settings,
            ILogFactory logFactory, ISubscriberRepository subscriberRepository,
            ISubscriberService subscriberService)
        {
            _settings = settings;
            _log = logFactory.CreateLog(this);
            _subscriberRepository = subscriberRepository;
            _subscriberService = subscriberService;
        }

        public async Task StartAsync()
        {
            await ExecuteAsync();
        }

        private async Task ExecuteAsync()
        {
            while (true)
            {
                var deactivatedSubscribers = await _subscriberRepository.GetSuscribersToDeactivateAsync();

                foreach (var deactivatedSubscriber in deactivatedSubscribers)
                {
                    await _subscriberService.DeactivateAccountAsync(deactivatedSubscriber.ClientId);
                }
                await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalInSeconds));
            }
        }
    }
}
