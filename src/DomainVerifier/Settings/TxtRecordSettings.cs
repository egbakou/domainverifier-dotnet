namespace DomainVerifier.Settings
{
    /// <summary>
    /// Settings used to give instructions to the user on how to create a TXT record.
    /// It is also used to verify that the TXT record has been created.
    /// </summary>
    public class TxtRecordSettings
    {
        /// <summary>
        /// The name of the TXT record.
        /// </summary>
        public string Hostname { get; set; } = "@";

        /// <summary>
        /// The content attribute of the TXT record.
        /// </summary>
        public string? RecordAttribute { get; set; }
    }
}