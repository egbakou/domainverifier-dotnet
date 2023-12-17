using DomainVerifier.Settings;
using System.Threading.Tasks;

namespace DomainVerifier.Interfaces
{
    /// <summary>
    /// DnsRecordsVerifier interface.
    /// </summary>
    public interface IDnsRecordsVerifier
    {
        /// <summary>
        /// Verifies that the TXT record is valid.
        /// </summary>
        /// <param name="domainName">The domain name to verify the TXT record for.</param>
        /// <param name="verificationCode">The TXT record to verify.</param>
        /// <param name="options">Config used to generate the TXT record.</param>
        /// <returns>True if the TXT record is valid, false otherwise.</returns>
        Task<bool> IsTxtRecordValidAsync(string domainName, string verificationCode, TxtRecordSettings? options = null);

        /// <summary>
        /// Verifies that the CNS record is valid.
        /// </summary>
        /// <param name="domainName">The domain name to verify the CNS record for.</param>
        /// <param name="verificationCode">The CNS record to verify.</param>
        /// <param name="options">Config used to generate the CNS record.</param>
        /// <returns>True if the CNS record is valid, false otherwise.</returns>
        Task<bool> IsCnameRecordValidAsync(string domainName, string verificationCode, CnameRecordSettings? options = null);
    }
}