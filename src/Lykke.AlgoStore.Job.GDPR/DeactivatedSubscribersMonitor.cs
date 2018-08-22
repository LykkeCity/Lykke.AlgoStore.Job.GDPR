using Common.Log;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
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
        private readonly ISubscriberService _subscriberService;

        public DeactivatedSubscribersMonitor(DeactivatedSubscribersMonitorSettings settings,
                                             ILogFactory logFactory,
                                             ISubscriberService subscriberService)
        {
            _settings = settings;
            _log = logFactory.CreateLog(this);
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
                var deactivatedSubscribers = await _subscriberService.GetSuscribersToDeactivateAsync();

                foreach (var deactivatedSubscriber in deactivatedSubscribers)
                {
                   _log.Info($"Deactivation process of user has started for clientId: {deactivatedSubscriber.ClientId}.");

                    await _subscriberService.DeactivateAccountAsync(deactivatedSubscriber.ClientId);

                    await _subscriberService.DeleteDeactivatedSubscriberRow(deactivatedSubscriber.ClientId);

                    _log.Info($"Deactivation process completed for clientId: {deactivatedSubscriber.ClientId}.");
                }

                await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalInSeconds));
            }
        }
    }
}
