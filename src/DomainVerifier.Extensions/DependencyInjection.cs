using System.Collections.Generic;
using System.Linq;
using System.Net;
using DnsClient;
using DomainVerifier.Interfaces;
using DomainVerifier.Services;
using DomainVerifier.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DomainVerifier.Extensions
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Add Domain Verifier services to DI container.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddDomainVerifierServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<DomainVerifierSettings>()
                .BindConfiguration("DomainVerifierSettings")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<DomainVerifierSettings>>().Value);

            var dnsServers = configuration.GetSection("DomainVerifierSettings:DnsServerSettings")
                .Get<List<DnsServerSettings>>();

            var filteredDnsServers = dnsServers?.Where(x => !string.IsNullOrWhiteSpace(x.IpAddress))
                .ToList();

            var nameServers = (filteredDnsServers?.Count > 0) switch
            {
                true => filteredDnsServers
                    .Select(x => new IPEndPoint(IPAddress.Parse(x.IpAddress), x.Port))
                    .ToArray(),
                _ => new[]
                {
                    NameServer.Cloudflare,
                    NameServer.GooglePublicDns
                }
            };
            var client = new LookupClient(nameServers);

            services.AddSingleton<ILookupClient>(client);
            services.AddTransient<IDnsRecordsGenerator, DnsRecordsGenerator>();
            services.AddScoped<IDnsRecordsVerifier, DnsRecordsVerifier>();
            return services;
        }
    }
}