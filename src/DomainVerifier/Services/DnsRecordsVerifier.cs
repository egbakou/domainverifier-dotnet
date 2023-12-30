using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using DomainVerifier.Helpers;
using DomainVerifier.Interfaces;
using DomainVerifier.Settings;

namespace DomainVerifier.Services
{
    /// <summary>
    /// This service checks if a domain is verified by looking at its DNS records.
    /// </summary>
    /// <example>
    /// A basic example of how to use this service:
    /// <code>
    /// var client = new LookupClient(NameServer.Cloudflare, NameServer.GooglePublicDns);
    /// var txtSettings = new TxtRecordSettings
    /// {
    ///    Hostname = "@",
    ///    RecordAttribute = "myapp-verification-code"
    /// };
    /// var cnameSettings = new CnameRecordSettings
    /// {
    ///    RecordTarget = "verify.my-app.com"
    /// };
    /// var settings = new DomainVerifierSettings(txtSettings, cnameSettings);
    /// var verifier = new DnsRecordsVerifier(client, settings);
    /// var domainName = "user-domain.com";
    /// var verificationCode = "random-verification-code"; // retrieved from database
    /// var isTxtRecordValid = verifier.IsTxtRecordValidAsync(domainName, verificationCode);
    /// </code>
    /// </example>
    public class DnsRecordsVerifier : IDnsRecordsVerifier
    {
        private readonly ILookupClient _lookupClient;
        private readonly TxtRecordSettings? _txtRecordSettings;
        private readonly CnameRecordSettings? _cnameRecordSettings;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DnsRecordsVerifier" /> class.
        /// </summary>
        /// <param name="lookupClient">Dns lookup client.</param>
        /// <param name="settings">The settings.</param>
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
            return txtRecords?.Any(txtRecord => txtRecord.Text.Any(x => x == record)) ?? false;
        }

        /// <inheritdoc />
        public async Task<bool> IsCnameRecordValidAsync(string domainName, string verificationCode,
            CnameRecordSettings? options = null)
        {
            ArgumentExceptionHelper.ThrowIfNullOrEmpty(options?.RecordTarget ?? _cnameRecordSettings?.RecordTarget,
                nameof(options.RecordTarget));

            var recordTarget = $"{options?.RecordTarget ?? _cnameRecordSettings?.RecordTarget}.";
            var hostQuery = $"{verificationCode}.{domainName}";

            var queryResponse = await _lookupClient.QueryAsync(hostQuery, QueryType.CNAME);

            if (queryResponse.HasError) return false;

            var cnameRecords = queryResponse.Answers.CnameRecords();
            return cnameRecords?.Any(cnameRecord => cnameRecord.CanonicalName == recordTarget) ?? false;
        }
    }
}