using System.Collections.Generic;

namespace DomainVerifier.Settings
{
    public class DomainVerifierSettings
    {
        public List<DnsServerSettings> DnsServerSettings { get; set; }

        public TxtRecordSettings TxtRecordSettings { get; set; }

        public CnameRecordSettings CnameRecordSettings { get; set; }


        public DomainVerifierSettings(List<DnsServerSettings> dnsServerSettings,
            TxtRecordSettings txtRecordSettings,
            CnameRecordSettings cnameRecordSettings)
        {
            DnsServerSettings = dnsServerSettings;
            TxtRecordSettings = txtRecordSettings;
            CnameRecordSettings = cnameRecordSettings;
        }
    }
}