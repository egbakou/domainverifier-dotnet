namespace DomainVerifier.Extensions
{
    /// <summary>
    /// Represents a DNS server.
    /// </summary>
    public class DnsServer
    {
        /// <summary>
        /// Gets or sets the name of the DNS server.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the DNS server.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the port of the DNS server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsServer"/> class.
        /// </summary>
        public DnsServer()
        {
            IpAddress = string.Empty;
            Port = 53;
        }
        
        /// <summary>
        ///  Initializes a new instance of the <see cref="DnsServer"/> class.
        /// </summary>
        /// <param name="ipAddress">The IP address of the DNS server.</param>
        /// <param name="port">The port of the DNS server.</param>
        /// <param name="name">The name of the DNS server.</param>
        public DnsServer(string ipAddress, int port, string? name)
        {
            Name = name;
            IpAddress = ipAddress;
            Port = port;
        }
    }
}