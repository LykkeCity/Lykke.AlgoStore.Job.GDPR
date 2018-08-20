using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Core.Utils;
using Lykke.AlgoStore.Job.GDPR.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Log;

namespace Lykke.AlgoStore.Job.GDPR.Controllers
{
    [Route("api/v1/subscribers")]
    public class SubscribersController : Controller
    {
        private readonly ISubscriberService _subscriberService;
        private readonly ILog _log;

        public SubscribersController(ISubscriberService subscriberService, ILogFactory logFactory)
        {
            _subscriberService = subscriberService;
            _log = logFactory.CreateLog(this);
        }

        [HttpGet("legalConsents")]
        [ProducesResponseType(typeof(SubscriberModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLegalConsents(string clientId)
        {
            var result = await _log.LogElapsedTimeAsync(clientId, async () => await _subscriberService.GetByIdAsync(clientId));

            return Ok(Mapper.Map<SubscriberModel>(result));
        }

        [HttpPost("gdprConsent")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> SetUserGdprConsent(string clientId)
        {
            await _log.LogElapsedTimeAsync(null, async () => await _subscriberService.SetGdprConsentAsync(clientId));

            return NoContent();
        }

        [HttpPost("cookieConsent")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> SetUserCookieConsent(string clientId)
        {
            await _log.LogElapsedTimeAsync(clientId, async () => await _subscriberService.SetCookieConsentAsync(clientId));

            return NoContent();
        }

        [HttpPost("deactivateAccount")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeactivateUserAccount(string clientId)
        {
            await _log.LogElapsedTimeAsync(clientId, async () => await _subscriberService.RemoveUserConsents(clientId));

            return NoContent();
        }

        [HttpGet("seedConsent")]
        [ProducesResponseType(typeof(SubscriberModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SeedConsent(string clientId)
        {
            await _log.LogElapsedTimeAsync(clientId, async () => await _subscriberService.SeedAsync(clientId));

            return NoContent();
        }
    }
}
