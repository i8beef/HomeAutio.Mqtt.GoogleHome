namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Signing certificate.
    /// </summary>
    public class SigningCertificate
    {
        /// <summary>
        /// File path.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Pass phrase for the certificate.
        /// </summary>
        public string PassPhrase { get; set; }
    }
}
