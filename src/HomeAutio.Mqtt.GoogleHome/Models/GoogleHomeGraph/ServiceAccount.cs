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
        public required string Type { get; init; }

        /// <summary>
        /// Project id.
        /// </summary>
        [JsonProperty(PropertyName = "project_id")]
        public required string ProjectId { get; init; }

        /// <summary>
        /// Private key id.
        /// </summary>
        [JsonProperty(PropertyName = "private_key_id")]
        public required string PrivateKeyId { get; init; }

        /// <summary>
        /// Private key.
        /// </summary>
        [JsonProperty(PropertyName = "private_key")]
        public required string PrivateKey { get; init; }

        /// <summary>
        /// Client email.
        /// </summary>
        [JsonProperty(PropertyName = "client_email")]
        public required string ClientEmail { get; init; }

        /// <summary>
        /// Client id.
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public required string ClientId { get; init; }

        /// <summary>
        /// Auth URI.
        /// </summary>
        [JsonProperty(PropertyName = "auth_uri")]
        public required string AuthUri { get; init; }

        /// <summary>
        /// Token URI.
        /// </summary>
        [JsonProperty(PropertyName = "token_uri")]
        public required string TokenUri { get; init; }

        /// <summary>
        /// Auth provider X509 cert url.
        /// </summary>
        [JsonProperty(PropertyName = "auth_provider_x509_cert_url")]
        public required string AuthProviderX509CertUrl { get; init; }

        /// <summary>
        /// Client X509 cert url.
        /// </summary>
        [JsonProperty(PropertyName = "client_x509_cert_url")]
        public required string ClientX509CertUrl { get; init; }
    }
}
