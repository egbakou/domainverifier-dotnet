using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace DomainVerifier.Tests;

public class DnsServerFixture : IAsyncLifetime
{
    private readonly IContainer _dnsServerContainer;

    public DnsServerFixture()
    {
        var port = GenerateRandomPort().ToString();
        _dnsServerContainer = new ContainerBuilder()
            .WithImage("ubuntu/bind9")
            .WithPortBinding(port, "53/tcp")
            .WithPortBinding(port, "53/udp")
            .WithEnvironment("BIND9_USER", "root")
            .WithEnvironment("TZ", "Europe/Paris")
            .WithResourceMapping("config/", "/etc/bind")
            .WithCleanUp(true)
            .Build();
    }

    public string Hostname => _dnsServerContainer.Hostname;
    public int HostPort => _dnsServerContainer.GetMappedPublicPort("53");
    public string ContainerName => _dnsServerContainer.Name;


    public Task InitializeAsync()
        => _dnsServerContainer.StartAsync();

    public Task DisposeAsync()
        => _dnsServerContainer.DisposeAsync().AsTask();

    private static int GenerateRandomPort()
    {
        var random = new Random();
        var port = random.Next(70, 9999);
        return port;
    }
}