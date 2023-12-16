using System;
using System.Net;
using System.Threading.Tasks;
using DnsClient;
using DomainVerifier.Interfaces;
using DomainVerifier.Services;
using DomainVerifier.Settings;
using Xunit.Abstractions;

namespace DomainVerifier.Tests;

public sealed class TxtRecordsVerifierTest(DnsServerFixture fixture, ITestOutputHelper output) :
    IClassFixture<DnsServerFixture>, IDisposable
{
    [Fact]
    public async Task IsTxtRecordValidAsync_WhenTxtRecordExists_Should_ReturnsTrue()
    {
        // Arrange
        const string domainNameToVerify = "localhost";
        const string verificationCode = "random000454";
        var settings = new TxtRecordSettings
        {
            Hostname = "@",
            RecordAttribute = "ownership-demo-app"
        };
        var endpoint = new IPEndPoint(IPAddress.Parse(fixture.Hostname), fixture.HostPort);

        var client = new LookupClient(endpoint);
        IDnsRecordsVerifier domainVerifierService = new DnsRecordsVerifier(client, null);

        // Act
        var result =
            await domainVerifierService.IsTxtRecordValidAsync(domainNameToVerify, verificationCode, settings);

        // Assert
        result.Should().Be(true);
    }


    [Fact]
    public async Task IsTxtRecordValidAsync_WhenTxtRecordExistsWithCustomHostName_Should_ReturnsTrue()
    {
        // Arrange
        const string domainNameToVerify = "localhost";
        const string verificationCode = "random000455";
        var settings = new TxtRecordSettings
        {
            Hostname = "ownership-demo-app-verification",
            RecordAttribute = null
        };
        var endpoint = new IPEndPoint(IPAddress.Parse(fixture.Hostname), fixture.HostPort);

        var client = new LookupClient(endpoint);
        IDnsRecordsVerifier domainVerifierService = new DnsRecordsVerifier(client, null);

        // Act
        var result =
            await domainVerifierService.IsTxtRecordValidAsync(domainNameToVerify, verificationCode, settings);

        // Assert
        result.Should().Be(true);
    }


    [Fact]
    public async Task IsTxtRecordValidAsync_WhenTxtRecordDoesNotExist_Should_ReturnsFalse()
    {
        // Arrange
        const string domainNameToVerify = "localhost";
        const string verificationCode = "non-existing-verification-code";
        var settings = new TxtRecordSettings
        {
            Hostname = "@",
            RecordAttribute = "ownership-demo-app"
        };
        var endpoint = new IPEndPoint(IPAddress.Parse(fixture.Hostname), fixture.HostPort);
        var client = new LookupClient(endpoint);
        var domainVerifierService = new DnsRecordsVerifier(client, null);

        // Act
        var result =
            await domainVerifierService.IsTxtRecordValidAsync(domainNameToVerify, verificationCode, settings);

        // Assert
        result.Should().Be(false);
    }


    [Fact]
    public async Task IsTxtRecordValidAsync_WhenTxtRecordSettingsIsNull_Should_ThrowException()
    {
        // Arrange
        const string domainNameToVerify = "localhost";
        const string verificationCode = "non-existing-verification-code";
        var settings = null as TxtRecordSettings;
        var endpoint = new IPEndPoint(IPAddress.Parse(fixture.Hostname), fixture.HostPort);
        var client = new LookupClient(endpoint);
        var domainVerifierService = new DnsRecordsVerifier(client, null);

        // Act
        Func<Task> func = async () =>
            await domainVerifierService.IsTxtRecordValidAsync(domainNameToVerify, verificationCode, settings);

        // Assert
        await func.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]
    public async Task IsTxtRecordValidAsync_WhenHostNameSettingsIsInValid_Should_ReturnsFalse()
    {
        // Arrange
        const string domainNameToVerify = "localhost";
        const string verificationCode = "non-existing-verification-code";
        var settings = new TxtRecordSettings
        {
            Hostname = "in#valid-hostname",
            RecordAttribute = "ownership-demo-app"
        };
        var endpoint = new IPEndPoint(IPAddress.Parse(fixture.Hostname), fixture.HostPort);
        var client = new LookupClient(endpoint);
        var domainVerifierService = new DnsRecordsVerifier(client, null);

        // Act
        var result =
            await domainVerifierService.IsTxtRecordValidAsync(domainNameToVerify, verificationCode, settings);

        // Assert
        result.Should().Be(false);
    }

    public void Dispose()
    {
        output.WriteLine($"Disposing DnsServerFixture with port {fixture.HostPort}");
        output.WriteLine($"Disposing DnsServerFixture with container name {fixture.ContainerName}");
    }
}