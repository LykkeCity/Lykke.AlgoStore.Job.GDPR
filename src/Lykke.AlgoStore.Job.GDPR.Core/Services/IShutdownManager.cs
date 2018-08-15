using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
