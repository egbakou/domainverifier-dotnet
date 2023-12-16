using System;
using System.Net;
using System.Threading.Tasks;
using DnsClient;
using DomainVerifier.Interfaces;
using DomainVerifier.Services;
using DomainVerifier.Settings;
using Xunit.Abstractions;

namespace DomainVerifier.Tests;

public sealed class CnameRecordsVerifierTest(DnsServerFixture fixture, ITestOutputHelper output) :
    IClassFixture<DnsServerFixture>, IDisposable
{
    [Fact]
    public async Task IsCnameRecordValidAsync_WhenCnameRecordExists_Should_ReturnsTrue()
    {
        // Arrange
        const string domainNameToVerify = "localhost";
        const string verificationCode = "random000454";
        var settings = new CnameRecordSettings("ownership-demo-app.com");

        var endpoint = new IPEndPoint(IPAddress.Parse(fixture.Hostname), fixture.HostPort);
        var client = new LookupClient(endpoint);

        IDnsRecordsVerifier domainVerifierService = new DnsRecordsVerifier(client, null);

        // Act
        var result =
            await domainVerifierService.IsCnameRecordValidAsync(domainNameToVerify, verificationCode, settings);

        // Assert
        result.Should().Be(true);
    }


    [Fact]
    public async Task IsCnameRecordValidAsync_WhenCnameRecordDoesNotExist_Should_ReturnsFalse()
    {
        // Arrange
        const string domainNameToVerify = "localhost";
        const string verificationCode = "not-existing-verification-code";
        var settings = new CnameRecordSettings("ownership-demo-app.com");

        var endpoint = new IPEndPoint(IPAddress.Parse(fixture.Hostname), fixture.HostPort);
        var client = new LookupClient(endpoint);

        IDnsRecordsVerifier domainVerifierService = new DnsRecordsVerifier(client, null);

        // Act
        var result =
            await domainVerifierService.IsCnameRecordValidAsync(domainNameToVerify, verificationCode, settings);

        // Assert
        result.Should().Be(false);
    }


    [Fact]
    public async Task IsCnameRecordValidAsync_WhenCnameRecordSettingsIsNull_Should_ThrowException()
    {
        // Arrange
        const string domainNameToVerify = "localhost";
        const string verificationCode = "not-existing-verification-code";
        CnameRecordSettings? settings = null;

        var endpoint = new IPEndPoint(IPAddress.Parse(fixture.Hostname), fixture.HostPort);
        var client = new LookupClient(endpoint);

        IDnsRecordsVerifier domainVerifierService = new DnsRecordsVerifier(client, null);

        // Act
        Func<Task> act = async () =>
            await domainVerifierService.IsCnameRecordValidAsync(domainNameToVerify, verificationCode, settings!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }


    public void Dispose()
    {
        output.WriteLine($"Disposing DnsServerFixture with port {fixture.HostPort}");
        output.WriteLine($"Disposing DnsServerFixture with container name {fixture.ContainerName}");
    }
}