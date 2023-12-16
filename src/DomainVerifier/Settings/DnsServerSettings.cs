namespace DomainVerifier.Settings
{
    public class DnsServerSettings
    {
        public string? Name { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public DnsServerSettings()
        {
            IpAddress = string.Empty;
            Port = 53;
        }
        public DnsServerSettings(string ipAddress, int port, string? name)
        {
            Name = name;
            IpAddress = ipAddress;
            Port = port;
        }
    }
}