namespace DomainVerifier.Settings
{
    /// <summary>
    /// Settings used to give instructions to the user on how to create a TXT or CNAME record.
    /// It is also used to verify that the TXT or CNAME record has been created.
    /// </summary>
    /// <seealso cref="TxtRecordSettings"/>
    /// <seealso cref="CnameRecordSettings"/>
    public class DomainVerifierSettings
    {
        /// <summary>
        /// TXT record settings.
        /// </summary>
        public TxtRecordSettings? TxtRecordSettings { get; set; }

        /// <summary>
        /// CNAME record settings.
        /// </summary>
        public CnameRecordSettings? CnameRecordSettings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainVerifierSettings"/> class.
        /// </summary>
        public DomainVerifierSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainVerifierSettings"/> class with
        /// the specified TXT record settings.
        /// </summary>
        /// <param name="txtRecordSettings">The TXT record settings.</param>
        /// <param name="cnameRecordSettings">The CNAME record settings.</param>
        public DomainVerifierSettings(TxtRecordSettings txtRecordSettings, CnameRecordSettings cnameRecordSettings)
        {
            TxtRecordSettings = txtRecordSettings;
            CnameRecordSettings = cnameRecordSettings;
        }
    }
}