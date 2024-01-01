namespace DomainVerifier.Settings
{
    /// <summary>
    /// Settings used to give instructions to the user on how to create a CNAME record.
    /// It is also used to verify that the CNAME record has been created.
    /// </summary>
    public class CnameRecordSettings
    {
        /// <summary>
        ///  The target of the CNAME record.
        /// </summary>
        public string? RecordTarget { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CnameRecordSettings"/> class.
        /// </summary>
        public CnameRecordSettings()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="CnameRecordSettings"/> class with the specified record target.
        /// </summary>
        /// <param name="recordTarget"></param>
        public CnameRecordSettings(string recordTarget)
        {
            RecordTarget = recordTarget;
        }
    }
}