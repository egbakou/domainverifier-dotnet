namespace DomainVerifier.Extensions.Tests;

public class DnsServerSettingsTest
{
    [Fact]
    public void Instantiate_DnsServerSettings_With_Parameters_Constructor_should_Have_Not_Nullable_Properties()
    {
        var dnsServer = new DnsServer("1.1.1.1.", 53, "Cloudflare");

        dnsServer.Name.Should().NotBeNull();
        dnsServer.Name.Should().Be("Cloudflare");
        dnsServer.IpAddress.Should().NotBeNull();
        dnsServer.Port.Should().Be(53);
    }
}