<div style="text-align:center"><img src="assets/logo.png" width=80 /></div>

[![CI](https://github.com/egbakou/domainverifier-dotnet/actions/workflows/ci-core.yml/badge.svg)](https://github.com/egbakou/domainverifier-dotnet/actions/workflows/ci-core.yml) [![CI](https://github.com/egbakou/domainverifier-dotnet/actions/workflows/ci-extensions.yml/badge.svg)](https://github.com/egbakou/domainverifier-dotnet/actions/workflows/ci-extensions.yml)

domainverifier-dotent  is a .NET library project designed to simplify domain name ownership verification. 

It consists of two projects: `DomainVerifier` and `DomainVerifier.Extensions`. 

The `DomainVerifier` project is a .NET Standard 2.1 project, making it compatible with .NET Core 3.0 to .NET 8. It provides core functionalities for generating domain verification codes and verifying domain ownership.

The `DomainVerifier.Extensions` project extends the functionality of `DomainChecker` to support .NET 6, 7, and 8 with dependency injection.

## DomainVerifier

### Installation

```shell
dotnet add package DomainVerifier
```

This package is intended exclusively for utilization in .NET projects with versions earlier than 6.0. If you are working with a .NET 6, 7, or 8 application, please install the `DomainVerifier.Extensions` package instead.

### Overview

The `DomainChecker` project includes a `DnsRecordsGenerator` class that implements the  as well as a `DnsRecordsVerifier` class that implements the `IDnsRecordsVerifier` interface. These classes offer convenient methods for generating domain verification codes and verifying ownership through DNS records(TXT and CNAME)

### DnsRecordsGenerator

It can be injected into into your service class or controller suing it interface or instantiated directly.

The `DnsRecordsGenerator` class provides the following methods:

#### `string GenerateDnsRecord(int length = 10)`

Generates a domain name verification code that users can add as TXT or CNAME records to prove ownership.

```c#
string verificationCode = dnsRecordsGenerator.GenerateDnsRecord(15);
```

#### `string GetTxtInstructions(string domainName, string verificationCode, TxtRecordSettings? options = null)`

This method provides users with instructions on adding the TXT record to their DNS for domain verification.

The `TxtRecordSettings` is a configuration class that includes the `Hostname` (representing the name of the TXT record) and `RecordAttribute` (optional content attribute of the TXT record). This configuration class is optional if the `DnsRecordsGenerator` has already been instantiated with the `DomainVerifierSettings`.

```c#
const string domainNameToVerify = "the-domain-to-verify.com";
const string verificationCode = "randomcode"; // generated using GenerateDnsRecord()
var settings = new TxtRecordSettings
{
	Hostname = "@", // or anything else but unique. example: _myappname-challenge-code 
	RecordAttribute = "my-appname-verification"
};

string instructions = dnsRecordsGenerator.GetTxtInstructions(domainNameToVerify,
                                                             verificationCode,
                                                             settings
                                                            )
```



#### `string GetCnameInstructions(string verificationCode, CnameRecordSettings? options = null)`

This method serves the same purpose as the previous one but for CNAME records.

It provides users with instructions on adding the CNAME record to their DNS settings.

The `CnameRecordSettings` is a configuration class that includes the `RecordTarget` (representing the target of the CNAME record). This configuration class is also optional if the `DnsRecordsGenerator` has already been instantiated with the `DomainVerifierSettings`.



