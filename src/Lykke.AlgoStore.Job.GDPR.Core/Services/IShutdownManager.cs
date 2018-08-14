using System.Threading.Tasks;

namespace Lykke.Job.GDPR.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
