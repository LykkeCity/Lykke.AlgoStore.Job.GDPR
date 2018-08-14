using System.Threading.Tasks;

namespace Lykke.Job.GDPR.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}