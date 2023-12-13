using DnsClient;
using DomainVerifier.Helpers;
using DomainVerifier.Interfaces;
using DomainVerifier.Settings;
using System.Linq;
using System.Threading.Tasks;

namespace DomainVerifier.Services
{
    public class DnsRecordsVerifier : IDnsRecordsVerifier
    {
        private readonly ILookupClient _lookupClient;
        private readonly TxtRecordSettings? _txtRecordSettings;
        private readonly CnameRecordSettings? _cnameRecordSettings;

        public DnsRecordsVerifier(ILookupClient lookupClient, DomainVerifierSettings? settings)
        {
            _txtRecordSettings = settings?.TxtRecordSettings;
            _cnameRecordSettings = settings?.CnameRecordSettings;
            _lookupClient = lookupClient;
        }

        /// <inheritdoc />
        public async Task<bool> IsTxtRecordValidAsync(string domainName, string verificationCode,
            TxtRecordSettings? options = null)
        {
            ArgumentExceptionHelper.ThrowIfNullOrEmpty(options?.Hostname ?? _txtRecordSettings?.Hostname,
                nameof(options.Hostname));

            var hostname = options?.Hostname ?? _txtRecordSettings?.Hostname;
            var recordAttribute = options?.RecordAttribute ?? _txtRecordSettings?.RecordAttribute;
            var record = recordAttribute switch
            {
                _ when !string.IsNullOrEmpty(recordAttribute) => $"{recordAttribute}={verificationCode}",
                _ => verificationCode
            };

            var hostQuery = (domainName, hostname) switch
            {
                var (domain, host) when host != "@" && host != domain => $"{host}.{domain}",
                var (domain, _) => domain
            };

            var queryResponse = await _lookupClient.QueryAsync(hostQuery, QueryType.TXT);

            if (queryResponse.HasError) return false;

            var txtRecords = queryResponse.Answers.TxtRecords();
            return txtRecords != null && txtRecords.Any(txtRecord => txtRecord.Text.Any(x => x == record));
        }

        /// <inheritdoc />
        public async Task<bool> IsCnameRecordValidAsync(string domainName, string verificationCode,
            CnameRecordSettings? options = null)
        {
            ArgumentExceptionHelper.ThrowIfNullOrEmpty(options?.RecordTarget ?? _cnameRecordSettings?.RecordTarget,
                nameof(options.RecordTarget));

            var recordTarget = options?.RecordTarget ?? _cnameRecordSettings?.RecordTarget;
            var hostQuery = $"{verificationCode}.{domainName}";

            var queryResponse = await _lookupClient.QueryAsync(hostQuery, QueryType.CNAME);

            if (queryResponse.HasError) return false;

            var cnameRecords = queryResponse.Answers.CnameRecords();
            return cnameRecords != null && cnameRecords.Any(cnameRecord => cnameRecord.CanonicalName == $"{recordTarget}.");
        }
    }
}