using DomainVerifier.Settings;

namespace DomainVerifier.Interfaces
{
    /// <summary>
    /// DnsRecordsGenerator interface.
    /// </summary>
    public interface IDnsRecordsGenerator
    {
        /// <summary>
        /// Generates a domain name verification code.
        /// </summary>
        /// <param name="length"> The desired length of the generated code (default is 10). </param>
        /// <returns> The generated code. </returns>
        string GenerateDnsRecord(int length = 10);

        /// <summary>
        /// Give the instructions to the user on how to add the TXT record to their DNS.
        /// </summary>
        /// <param name="domainName"> The domain name to generate the instructions for. </param>
        /// <param name="verificationCode"> The verification code to add to the TXT record. </param>
        /// <param name="options"> The additional options to use to set up the TXT record.
        /// If null, the config defined in appsettings.json or at instantiation will be used. </param>
        /// <returns>The instructions to the user on how to add the TXT record to their DNS. </returns>
        string GetTxtInstructions(string domainName, string verificationCode, TxtRecordSettings? options = null);

        /// <summary>
        /// Give the instructions to the user on how to add the CNAME record to their DNS.
        /// </summary>
        /// <param name="verificationCode"> The verification code to add to the CNAME record. </param>
        /// <param name="options"> The additional options to use to set up the CNAME record.
        /// If null, the config defined in appsettings.json or at instantiation will be used. </param>
        /// <returns>The instructions to the user on how to add the CNAME record to their DNS. </returns>
        string GetCnameInstructions(string verificationCode, CnameRecordSettings? options = null);
    }
}