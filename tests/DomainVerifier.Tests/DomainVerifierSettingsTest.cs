using DomainVerifier.Settings;

namespace DomainVerifier.Tests;

public class DomainVerifierSettingsTest
{
    [Fact]
    public void Instantiate_DomainVerifierSettings_With_Default_Constructor_should_Have_Nullable_Properties()
    {
        var domainVerifierSettings = new DomainVerifierSettings();

        domainVerifierSettings.TxtRecordSettings.Should().BeNull();
        domainVerifierSettings.CnameRecordSettings.Should().BeNull();
    }


    [Fact]
    public void Instantiate_DomainVerifierSettings_With_Parameters_Constructor_should_Have_Not_Nullable_Properties()
    {
        var txtRecordSettings = new TxtRecordSettings();
        var cnameRecordSettings = new CnameRecordSettings("recordTarget");

        var domainVerifierSettings =
            new DomainVerifierSettings(txtRecordSettings, cnameRecordSettings);

        domainVerifierSettings.TxtRecordSettings.Should().NotBeNull();
        domainVerifierSettings.TxtRecordSettings?.Hostname.Should().Be("@");
        domainVerifierSettings.CnameRecordSettings.Should().NotBeNull();
    }
}