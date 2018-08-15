﻿using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.Common.Log;

namespace Lykke.AlgoStore.Job.GDPR.Services
{
    // NOTE: Sometimes, startup process which is expressed explicitly is not just better, 
    // but the only way. If this is your case, use this class to manage startup.
    // For example, sometimes some state should be restored before any periodical handler will be started, 
    // or any incoming message will be processed and so on.
    // Do not forget to remove As<IStartable>() and AutoActivate() from DI registrations of services, 
    // which you want to startup explicitly.

    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;

        public StartupManager(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
        }

        public async Task StartAsync()
        {
            // TODO: Implement your startup logic here. Good idea is to log every step
            _log.Info("Lykke AlgoStore GDPR Job is starting...");

            await Task.CompletedTask;
        }
    }
}
