using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph
{
    /// <summary>
    /// Google service account.
    /// </summary>
    public class ServiceAccount
    {
        /// <summary>
        /// Account type.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Project id.
        /// </summary>
        [JsonProperty(PropertyName = "project_id")]
        public string ProjectId { get; set; }

        /// <summary>
        /// Private key id.
        /// </summary>
        [JsonProperty(PropertyName = "private_key_id")]
        public string PrivateKeyId { get; set; }

        /// <summary>
        /// Private key.
        /// </summary>
        [JsonProperty(PropertyName = "private_key")]
        public string PrivateKey { get; set; }

        /// <summary>
        /// Client email.
        /// </summary>
        [JsonProperty(PropertyName = "client_email")]
        public string ClientEmail { get; set; }

        /// <summary>
        /// Client id.
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Auth URI.
        /// </summary>
        [JsonProperty(PropertyName = "auth_uri")]
        public string AuthUri { get; set; }

        /// <summary>
        /// Token URI.
        /// </summary>
        [JsonProperty(PropertyName = "token_uri")]
        public string TokenUri { get; set; }

        /// <summary>
        /// Auth provider X509 cert url.
        /// </summary>
        [JsonProperty(PropertyName = "auth_provider_x509_cert_url")]
        public string AuthProviderX509CertUrl { get; set; }

        /// <summary>
        /// Client X509 cert url.
        /// </summary>
        [JsonProperty(PropertyName = "client_x509_cert_url")]
        public string ClientX509CertUrl { get; set; }
    }
}
