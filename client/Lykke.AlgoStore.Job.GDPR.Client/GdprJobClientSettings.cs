using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Job.GDPR.Client
{
    /// <summary>
    /// Gdpr job client settings
    /// </summary>
    public class GdprJobClientSettings
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
