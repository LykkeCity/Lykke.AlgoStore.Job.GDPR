using Common.Log;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lykke.Common.Log;

namespace Lykke.AlgoStore.Job.GDPR.Core.Utils
{
    public static class LogExtensions
    {
        public static async Task LogElapsedTimeAsync(this ILog log, string clientId, Func<Task> action)
        {
            if (action == null)
                return;

            var methodName = action.Method.Name;
            var context = action.Method.DeclaringType.FullName;

            var sw = new Stopwatch();
            sw.Start();
            var hasError = false;

            try
            {
                await action();
            }
            catch (Exception)
            {
                hasError = true;
                throw;
            }
            finally
            {
                var elapsed = sw.ElapsedMilliseconds;
                var message = $"Client {clientId} executed {methodName} for {elapsed}ms with HasError={hasError}";

                log.Info(Constants.ComponentName, message, context);
            }
        }

        public static async Task<T> LogElapsedTimeAsync<T>(this ILog log, string clientId, Func<Task<T>> action)
        {
            if (action == null)
                return default(T);

            var methodName = action.Method.Name;
            var context = action.Method.DeclaringType.FullName;

            var sw = new Stopwatch();
            sw.Start();
            var hasError = false;

            try
            {
                return await action();
            }
            catch (Exception)
            {
                hasError = true;
                throw;
            }
            finally
            {
                var elapsed = sw.ElapsedMilliseconds;
                var message = $"Client {clientId} executed {methodName} for {elapsed}ms with HasError={hasError}";

                log.Info(Constants.ComponentName, message, context);
            }
        }
    }
}
