using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DomainVerifier.Helpers;
using DomainVerifier.Interfaces;
using DomainVerifier.Settings;

namespace DomainVerifier.Services
{
    /// <summary>
    /// This class serves as a helper for generating DNS records required for domain verification.
    /// In addition to record generation, it offers convenient methods to display instructions
    /// guiding users through the process of adding these DNS records to their DNS configuration.
    /// </summary>
    /// <example>
    /// A basic example of how to use this helper:
    /// <code>
    /// var generator = new DnsRecordsGenerator();
    /// var verificationCode = generator.GenerateDnsRecord(15);
    /// </code>
    /// </example>
    public sealed class DnsRecordsGenerator : IDnsRecordsGenerator
    {
        private readonly TxtRecordSettings? _txtRecordSettings;
        private readonly CnameRecordSettings? _cnameRecordSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsRecordsGenerator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public DnsRecordsGenerator(DomainVerifierSettings settings)
        {
            _txtRecordSettings = settings.TxtRecordSettings;
            _cnameRecordSettings = settings.CnameRecordSettings;
        }

        /// <inheritdoc />
        public string GenerateDnsRecord(int length = 10)
        {
            ArgumentExceptionHelper.ThrowIfNotLongEnough(length, 10, nameof(length));
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            var randomBase64String = Convert.ToBase64String(bytes);
            return Regex.Replace(randomBase64String.ToLower(), "[^a-zA-Z0-9]", "")[..length];
        }

        /// <inheritdoc />
        public string GetTxtInstructions(string domainName, string verificationCode, TxtRecordSettings? options = null)
        {
            ArgumentExceptionHelper.ThrowIfNullOrEmpty(options?.Hostname ?? _txtRecordSettings?.Hostname,
                nameof(options.Hostname));

            var instructions = new StringBuilder();
            instructions.AppendLine("Add a DNS TXT record");
            instructions.AppendLine("1. Create a TXT record in your DNS configuration for the following hostname:");

            var hostname = options?.Hostname ?? _txtRecordSettings?.Hostname;
            instructions.AppendLine((hostname, domainName) switch
            {
                var (host, domain) when host != "@" && host != domain => $"{host}.{domain}",
                var (_, domain) => $"@ or {domain}"
            });
            instructions.AppendLine("2. Use this code for the value of the TXT record:");

            var recordAttribute = options?.RecordAttribute ?? _txtRecordSettings?.RecordAttribute;
            instructions.AppendLine((recordAttribute, verificationCode) switch
            {
                var (attribute, code) when !string.IsNullOrEmpty(attribute) => $"{attribute}={code}",
                var (_, code) => code
            });
            instructions.AppendLine(
                "Wait until your DNS configuration changes. This could take up to 72 hours to propagate.");
            return instructions.ToString();
        }

        /// <inheritdoc />
        public string GetCnameInstructions(string verificationCode, CnameRecordSettings? options = null)
        {
            ArgumentExceptionHelper.ThrowIfNullOrEmpty(options?.RecordTarget ?? _cnameRecordSettings?.RecordTarget,
                nameof(options.RecordTarget));

            var recordTarget = options?.RecordTarget ?? _cnameRecordSettings?.RecordTarget;
            return $"Add CNAME (alias) record with name {verificationCode} and value {recordTarget}.";
        }
    }
}