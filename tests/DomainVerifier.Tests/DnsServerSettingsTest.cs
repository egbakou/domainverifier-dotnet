using DomainVerifier.Settings;

namespace DomainVerifier.Tests;

public class DnsServerSettingsTest
{
    [Fact]
    public void Instantiate_DnsServerSettings_With_Parameters_Constructor_should_Have_Not_Nullable_Properties()
    {
        var dnsServerSettings = new DnsServerSettings("1.1.1.1.", 53, "Cloudflare");

        dnsServerSettings.Name.Should().NotBeNull();
        dnsServerSettings.Name.Should().Be("Cloudflare");
        dnsServerSettings.IpAddress.Should().NotBeNull();
        dnsServerSettings.Port.Should().Be(53);
    }
}