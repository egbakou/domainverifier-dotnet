using System.Linq;
using DnsClient;
using DomainVerifier.Interfaces;
using DomainVerifier.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DomainVerifier.Extensions.Tests;

public class DependencyInjectionTest
{
    private readonly ServiceCollection _services;

    public DependencyInjectionTest()
    {
        _services = new ServiceCollection();
        // load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // add IConfiguration to DI container
        _services.AddSingleton<IConfiguration>(configuration);

        // add DomainVerifier services to DI container
        _services.AddDomainVerifierServices(configuration);
    }


    [Fact]
    public void AddDomainVerifierServices_ShouldAddServices()
    {
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<ILookupClient>().Should().NotBeNull();
        serviceProvider.GetService<IDnsRecordsGenerator>().Should().NotBeNull();
        serviceProvider.GetService<IDnsRecordsVerifier>().Should().NotBeNull();
    }

    [Fact]
    public void AddDomainVerifierServices_ShouldInjectDomainVerifierSettings()
    {
        var serviceProvider = _services.BuildServiceProvider();
        var settings = serviceProvider.GetService<DomainVerifierSettings>();
        var client = serviceProvider.GetService<ILookupClient>();

        // Assert
        settings.Should().NotBeNull();
        
        client.Should().NotBeNull();
        client!.NameServers.Should().NotBeNull();
        client.NameServers!.Count.Should().Be(1);
        client.NameServers.First().Address.Should().Be("127.0.0.1");
            
        settings!.TxtRecordSettings.Should().NotBeNull();
        settings.TxtRecordSettings!.Hostname.Should().Be("@");
        
        settings.CnameRecordSettings.Should().NotBeNull();
        settings.CnameRecordSettings!.RecordTarget.Should().Be("ownership-demo-app.com");
    }
}