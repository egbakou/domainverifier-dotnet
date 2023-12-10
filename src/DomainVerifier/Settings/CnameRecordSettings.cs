namespace DomainVerifier.Settings
{
    public class CnameRecordSettings
    {
        public string RecordTarget { get; set; }
        
        public CnameRecordSettings(string recordTarget)
        {
            RecordTarget = recordTarget;
        }
    }
}