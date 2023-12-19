<div style="text-align:center"><img src="assets/logo.png" width=80 /></div>

[![CI](https://github.com/egbakou/domainverifier-dotnet/actions/workflows/ci-core.yml/badge.svg)](https://github.com/egbakou/domainverifier-dotnet/actions/workflows/ci-core.yml) [![CI](https://github.com/egbakou/domainverifier-dotnet/actions/workflows/ci-extensions.yml/badge.svg)](https://github.com/egbakou/domainverifier-dotnet/actions/workflows/ci-extensions.yml)

domainverifier-dotent  is a .NET library project designed to simplify domain name ownership verification. 

It consists of two projects: `DomainVerifier` and `DomainVerifier.Extensions`. 

The `DomainVerifier` project is a .NET Standard 2.1 project, making it compatible with .NET Core 3.0 to .NET 8. It provides core functionalities for generating domain verification codes and verifying domain ownership.

The `DomainVerifier.Extensions` project extends the functionality of `DomainChecker` to support .NET 6, 7, and 8 with dependency injection.

## DomainVerifier (The core library)

### Installation

```console
dotnet add package DomainVerifier
```

> This package is intended exclusively for utilization in .NET projects with versions earlier than 6.0. 

If you are working with a .NET 6, 7, or 8 application, please install the `DomainVerifier.Extensions` package instead like follow:

```console
dotnet add package DomainVerifier.Extensions
```

### Overview

The `DomainChecker` project includes a `DnsRecordsGenerator` class that implements the `DnsRecordsGenerator`  as well as a `DnsRecordsVerifier` class that implements the `IDnsRecordsVerifier` interface. These classes offer convenient methods for generating domain verification codes and verifying ownership through DNS records(TXT and CNAME)

### 🔑 DnsRecordsGenerator 

It can be injected into into your service class or controller suing it interface or instantiated directly.

The `DnsRecordsGenerator` class provides the following methods:

#### `string GenerateDnsRecord(int length = 10)`

Generates a domain name verification code that users can add as TXT or CNAME records to prove ownership.

```csharp
string verificationCode = _dnsRecordsGenerator.GenerateDnsRecord(15);
```

#### `string GetTxtInstructions(string domainName, string verificationCode, TxtRecordSettings? options = null)`

This method provides users with instructions on adding the TXT record to their DNS for domain verification.

The `TxtRecordSettings` is a configuration class that includes the `Hostname` (representing the name of the TXT record) and `RecordAttribute` (optional content attribute of the TXT record). This configuration class is optional if the `DnsRecordsGenerator` has already been instantiated with the `DomainVerifierSettings`.

```csharp
const string domainNameToVerify = "the-domain-to-verify.com";
const string verificationCode = "randomcode"; // generated using GenerateDnsRecord()
var config = new TxtRecordSettings
{
	Hostname = "@", // or anything else but unique. example: _myappname-challenge-code 
	RecordAttribute = "my-appname-verification" // optional
};

string instructions = _dnsRecordsGenerator.GetTxtInstructions(domainNameToVerify, verificationCode, config)
```

> The verification code needs to be stored in the database alongside the corresponding domain name. Regarding configuration, placing it in the `appsettings.json` file is a straightforward option. However, it is crucial to ensure that the verification code remains constant as long as there are unverified domains stored in the database.

#### `string GetCnameInstructions(string verificationCode, CnameRecordSettings? options = null)`

This method serves the same purpose as the previous one but for CNAME records.

It provides users with instructions on adding the CNAME record to their DNS settings.

The `CnameRecordSettings` is a configuration class that includes the `RecordTarget` (representing the target of the CNAME record). This configuration class is also optional if the `DnsRecordsGenerator` has already been instantiated with the `DomainVerifierSettings`.

```csharp
const string domainNameToVerify = "the-domain-to-verify.com";
const string verificationCode = "randomcode"; // generated using GenerateDnsRecord()
var config = new CnameRecordSettings("verify.myappname.com");

string instructions = _dnsRecordsGenerator.GetCnameInstructions(domainNameToVerify, verificationCode, config)
```



### 🔍DnsRecordsVerifier

The `DnsRecordsVerifier` class is  a service designed to validate whether verification codes are configured in the DNS settings of a given domain name, thereby establishing ownership proof.

It has two method `IsTxtRecordValidAsync` and `IsCnameRecordValidAsync` that return true or false wheatear the ownership is verified.

#### `async Task<bool> IsTxtRecordValidAsync(string domainName, string verificationCode, TxtRecordSettings? options = null)`

Asynchronously checks if the TXT record is valid for the specified domain and verification code.

```csharp
const string domainName = "the-domain-to-verify.com"; // retrieved from database
const string verificationCode = "randomcode"; // retrieved from database
var config = new TxtRecordSettings
{
	Hostname = "@", // Load from configurations
	RecordAttribute = "my-appname-verification" // Load from configurations
};

var isOwnershipVerified = _dnsRecordsVerifier.IsTxtRecordValidAsync(domainName, verificationCode, config);

// Update your database to mark the domain as verified
```

#### `async Task<bool> IsCnameRecordValidAsync(string domainName, string verificationCode, CnameRecordSettings? options = null)`

Asynchronously checks if the CNAME record is valid for the specified domain and verification code.

```csharp
const string domainName = "the-domain-to-verify.com"; // retrieved from database
const string verificationCode = "randomcode"; // retrieved from database
var config = new CnameRecordSettings
{
	RecordTarget = "verify.myappname.com", // Load from configurations
};

var isOwnershipVerified = _dnsRecordsVerifier.IsCnameRecordValidAsync(domainName, verificationCode, config);

// Update your database to mark the domain as verified
```

#### 🌟 Best Practices

- Optimize the verification process by executing it in the background using .NET Hosted services or Background services. Additionally, provide an endpoint for users to trigger the verification process.
- In scenarios where multiple users attempt to claim ownership of a specific domain, generate a unique verification code for each of them. The rightful owner will be able to prove ownership using the respective code.



## DomainVerifier.Extensions (The recommended way)

This package extends the functionality of `DomainVerifier` to support .NET 6, 7, and 8 with dependency injection.

### Instructions

**Step 1:** Install the `DomainVerifier.Extensions` [NuGet package](https://www.nuget.org/packages/DomainVerifier.Extensions) in your application.

```typescript
dotnet add package DomainVerifier.Extensions
```

**Step 2:** Add the verification configuration to your `appsettings.json`:

```json
{
  "DomainVerifierSettings": {
    "DnsServers": [ // Optional
      {
        "Name": "My Custom DNS Server Name",
        "IpAddress": "127.0.0.1",
        "Port": 53
      }
    ],
    "TxtRecordSettings": {
      "Hostname": "@",
      "RecordAttribute": "my-appname-verification"
    },
    "CnameRecordSettings": {
      "RecordTarget": "verify.myappname.com"
    }
  }
}
```

**Step 3:** Add the Domain verifier service to the service collection:

```csharp
services.AddDomainVerifierService(configuration);
// Or
builder.Services.AddDomainVerifierService(builder.Configuration);
```

`DnsRecordsGenerator` and `DnsRecordsVerifier` are already registered with the dependency injection containers.

### Usage:

Simply inject the `IDnsRecordsGenerator` or `IDnsRecordsVerifier` interface into your service class or controller.



## Contributing

Contributions to domainverifier-dotnet are very welcome. For guidance, please see [CONTRIBUTING.md](https://github.com/egbakou/domainverifier-dotnet/blob/main/CONTRIBUTING.md)



## Created by: Laurent Egbakou

- Twitter: [@lioncoding](https://twitter.com/lioncoding)
- LinkedIn: [Laurent Egbakou](https://www.linkedin.com/in/laurentegbakou/)



## Frequently Asked Questions

- **Q:** I am using .NET Core 3 - .NET 5 web API or web app. How do I use this library?

  > **A:** Refer to the [AddDomainVerifierService(configuration)](https://github.com/egbakou/domainverifier-dotnet/blob/main/src/DomainVerifier.Extensions/DependencyInjection.cs#L25C56-L25C56) method and replicate it in your target framework.

- **Q:** Can I use this library in my commercial project?

  > **A:** Yes.
