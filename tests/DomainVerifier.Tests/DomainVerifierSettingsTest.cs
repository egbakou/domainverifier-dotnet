using System.Collections.Generic;
using DomainVerifier.Settings;

namespace DomainVerifier.Tests;

public class DomainVerifierSettingsTest
{
    [Fact]
    public void Instantiate_DomainVerifierSettings_With_Default_Constructor_should_Have_Nullable_Properties()
    {
        var domainVerifierSettings = new DomainVerifierSettings();

        domainVerifierSettings.DnsServerSettings.Should().BeNull();
        domainVerifierSettings.TxtRecordSettings.Should().BeNull();
        domainVerifierSettings.CnameRecordSettings.Should().BeNull();
    }


    [Fact]
    public void Instantiate_DomainVerifierSettings_With_Parameters_Constructor_should_Have_Not_Nullable_Properties()
    {
        var dnsServerSettings = new List<DnsServerSettings>();
        var txtRecordSettings = new TxtRecordSettings();
        var cnameRecordSettings = new CnameRecordSettings("recordTarget");

        var domainVerifierSettings =
            new DomainVerifierSettings(dnsServerSettings, txtRecordSettings, cnameRecordSettings);

        domainVerifierSettings.DnsServerSettings.Should().NotBeNull();
        domainVerifierSettings.TxtRecordSettings.Should().NotBeNull();
        domainVerifierSettings.TxtRecordSettings?.Hostname.Should().Be("@");
        domainVerifierSettings.CnameRecordSettings.Should().NotBeNull();
    }
}