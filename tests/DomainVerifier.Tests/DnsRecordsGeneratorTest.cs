using System;
using DomainVerifier.Interfaces;
using DomainVerifier.Services;
using DomainVerifier.Settings;

namespace DomainVerifier.Tests;

public sealed class DnsRecordsGeneratorTest
{
    [Fact]
    public void GenerateDnsRecord_WhenLengthIsLessThan10_Should_ThrowsArgumentException()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        Action act = () => generator.GenerateDnsRecord(9);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The parameter 'length' cannot be less than 10.");
    }

    [Fact]
    public void GenerateDnsRecord_WhenLengthIsY_Should_ReturnYCharacters()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        var record = generator.GenerateDnsRecord();

        // Assert
        record.Should().NotBeNullOrEmpty();
        record.Should().HaveLength(10);
        record.Should().MatchRegex("[a-zA-Z0-9]");
    }

    [Fact]
    public void GenerateMultipleDnsRecords_Should_ReturnUniqueRecords()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        var record1 = generator.GenerateDnsRecord();
        var record2 = generator.GenerateDnsRecord();

        // Assert
        record1.Should().NotBeNullOrEmpty();
        record1.Should().HaveLength(10);
        record2.Should().NotBeNullOrEmpty();
        record2.Should().HaveLength(10);
        record1.Should().NotBe(record2);
    }


    [Fact]
    public void GetTxtInstructions_WhenHostNameIsNullOrEmpty_Should_ThrowsArgumentException()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        Action act = () => generator.GetTxtInstructions("domain.com", "code");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The parameter 'hostname' cannot be null or empty.");
    }


    [Fact]
    public void GetTxtInstructions_WhenHostNameIsAt_Should_ReturnsInstructions()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        var instructions = generator.GetTxtInstructions("domain.com",
            "code",
            new TxtRecordSettings
            {
                Hostname = "@"
            });

        // Assert
        instructions.Should().NotBeNullOrEmpty();
        instructions.Should().Contain("@ or domain.com");
        instructions.Should().Contain("1.");
    }


    [Fact]
    public void GetTxtInstructions_WhenHostNameIsDomain_Should_ReturnInstructions()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        var instructions = generator.GetTxtInstructions("domain.com",
            "code",
            new TxtRecordSettings
            {
                Hostname = "domain.com"
            });

        // Assert
        instructions.Should().NotBeNullOrEmpty();
        instructions.Should().Contain("@ or domain.com");
        instructions.Should().Contain("1.");
        instructions.Should().Contain("2.");
    }

    [Fact]
    public void GetTxtInstructions_WhenHostNameIsValid_Should_ReturnInstructions()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        var instructions = generator.GetTxtInstructions("domain.com",
            "code",
            new TxtRecordSettings
            {
                Hostname = "custom-hostname"
            });

        // Assert
        instructions.Should().NotBeNullOrEmpty();
        instructions.Should().Contain("custom-hostname");
        instructions.Should().Contain("1.");
        instructions.Should().Contain("2.");
    }


    [Fact]
    public void GetTxtInstructions_WhenRecordAttributeIsNotNullOrEmpty_Should_ReturnInstructions()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        var instructions = generator.GetTxtInstructions("domain.com",
            "code",
            new TxtRecordSettings
            {
                RecordAttribute = "custom-attribute"
            });

        // Assert
        instructions.Should().NotBeNullOrEmpty();
        instructions.Should().Contain("custom-attribute=code");
        instructions.Should().Contain("1.");
        instructions.Should().Contain("2.");
    }


    [Fact]
    public void GetTxtInstructions_WhenRecordAttributeIsNullOrEmpty_Should_ReturnInstructions()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        var instructions = generator.GetTxtInstructions("domain.com",
            "code",
            new TxtRecordSettings
            {
                RecordAttribute = string.Empty
            });

        // Assert
        instructions.Should().NotBeNullOrEmpty();
        instructions.Should().Contain("code");
        instructions.Should().Contain("1.");
        instructions.Should().Contain("2.");
    }


    [Fact]
    public void GetCnameInstructions_WhenRecordTargetIsNullOrEmpty_Should_ThrowsArgumentException()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        Action act = () => generator.GetCnameInstructions("code");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The parameter 'recordTarget' cannot be null or empty.");
    }


    [Fact]
    public void GetCnameInstructions_WhenRecordTargetIsValid_Should_ReturnInstructions()
    {
        // Arrange
        IDnsRecordsGenerator generator = new DnsRecordsGenerator(new DomainVerifierSettings());

        // Act
        var instructions = generator.GetCnameInstructions("code",
            new CnameRecordSettings("custom-target"));

        // Assert
        instructions.Should().NotBeNullOrEmpty();
        instructions.Should().Be("Add CNAME (alias) record with name code and value custom-target.");
    }
}