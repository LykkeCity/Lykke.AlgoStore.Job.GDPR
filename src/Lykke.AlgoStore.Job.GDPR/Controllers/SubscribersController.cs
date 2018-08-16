using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Job.GDPR.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Core.Utils;
using Lykke.AlgoStore.Job.GDPR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Job.GDPR.Controllers
{
    [Authorize]
    [Route("api/v1/subscribers")]
    public class SubscribersController: Controller
    {
        private readonly ISubscriberService _usersService;
        private readonly ILog _log;

        public SubscribersController(ISubscriberService usersService, ILog log)
        {
            _usersService = usersService;
            _log = log;
        }

        [HttpGet("legalConsents")]
        [ProducesResponseType(typeof(SubscriberModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLegalConsents(string clientId)
        {
            var result = await _log.LogElapsedTimeAsync(clientId, async () => await _usersService.GetByIdAsync(clientId));

            return Ok(Mapper.Map<SubscriberModel>(result));
        }

        [HttpPost("gdprConsent")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> SetUserGdprConsent([FromBody] string clientId)
        {
            await _log.LogElapsedTimeAsync(null, async () => await _usersService.SetGdprConsentAsync(clientId));

            return NoContent();
        }

        [HttpPost("cookieConsent")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> SetUserCookieConsent([FromBody] string clientId)
        {
            await _log.LogElapsedTimeAsync(clientId, async () => await _usersService.SetCookieConsentAsync(clientId));

            return NoContent();
        }

        [HttpPost("deactivateAccount")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeactivateUserAccount([FromBody] string clientId)
        {
            await _log.LogElapsedTimeAsync(clientId, async () => await _usersService.DeactivateAccountAsync(clientId));

            return NoContent();
        }
    }
}
