using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities
{
    public class UserData
    {
        public string ClientId { get; set; }
        public bool GDPRConsent { get; set; }
        public bool CookieConsent { get; set; }
    }
}
