using System;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System.Threading.Tasks;

namespace DomainVerifier.Tests;

public class DnsServerFixture : IAsyncLifetime
{
    private static readonly string Port = GenerateRandomPortAsString();
    private readonly IContainer _dnsServerContainer = new ContainerBuilder()
        .WithImage("ubuntu/bind9")
        .WithPortBinding(Port, "53/tcp")
        .WithPortBinding(Port, "53/udp")
        .WithEnvironment("BIND9_USER", "root")
        .WithEnvironment("TZ", "Europe/Paris")
        .WithResourceMapping("config/", "/etc/bind")
        .WithCleanUp(true)
        .WithStartupCallback((_, ct) => Task.CompletedTask)
        .Build();

    public string Hostname => _dnsServerContainer.Hostname;
    public int HostPort => _dnsServerContainer.GetMappedPublicPort("53");
    public string ContainerName => _dnsServerContainer.Name;


    public Task InitializeAsync()
        => _dnsServerContainer.StartAsync();

    public Task DisposeAsync()
         => _dnsServerContainer.DisposeAsync().AsTask();


    private static string GenerateRandomPortAsString()
    {
        var random = new Random();
        var port = random.Next(70, 9999);
        return port.ToString();
    }
}
