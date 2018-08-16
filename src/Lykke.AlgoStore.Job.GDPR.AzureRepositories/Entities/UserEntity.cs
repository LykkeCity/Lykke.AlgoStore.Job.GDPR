using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities
{
    public class UserEntity : TableEntity
    {
        public bool GDPRConsent { get; set; }
        public bool CookieConsent { get; set; }
        public DeletionStatus DeletionStatus { get; set; }
    }
}
