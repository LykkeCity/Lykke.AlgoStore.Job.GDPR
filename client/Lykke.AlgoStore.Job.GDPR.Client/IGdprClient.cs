using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.AlgoStore.Job.GDPR.Client.Model;
using Refit;

namespace Lykke.AlgoStore.Job.GDPR.Client
{
    /// <summary>
    /// GDPR client interface
    /// </summary>
    [PublicAPI]
    public interface IGdprClient
    {
        [Get("/api/v1/subscribers/legalConsents")]
        Task<Subscriber> GetLegalConsentsAsync(string clientId);

        [Post("/api/v1/subscribers/gdprConsent")]
        Task SetUserGdprConsentAsync(string clientId);

        [Post("/api/v1/subscribers/cookieConsent")]
        Task SetUserCookieConsentAsync(string clientId);

        [Post("/api/v1/subscribers/deactivateAccount")]
        Task DeactivateUserAccountAsync(string clientId);
    }
}
