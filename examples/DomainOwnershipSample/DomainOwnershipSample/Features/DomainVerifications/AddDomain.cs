using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Carter.ModelBinding;
using DomainOwnershipSample.Database.Context;
using DomainOwnershipSample.Entities;
using DomainVerifier.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DomainOwnershipSample.Features.DomainVerifications;

public record AddDomainRequest(string DomainName);

public record AddDomainNameResponse(string Txt, string Cname);

public class AddDomainNameRequestValidator : AbstractValidator<AddDomainRequest>
{
    public AddDomainNameRequestValidator()
    {
        RuleFor(x => x.DomainName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Domain name is required");
        RuleFor(x => x.DomainName)
            .Matches(@"^(?!:\/\/)([a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$")
            .WithMessage("Invalid domain name");
    }
}

public static class AddDomain
{
    public static async Task<Results<Created<AddDomainNameResponse>, BadRequest<string>>>
        Handler(
            HttpContext httpContext,
            ApplicationDbContext dbContext,
            ClaimsPrincipal claimsPrincipal,
            IDnsRecordsGenerator dnsRecordsGenerator,
            [FromBody] AddDomainRequest request
        )
    {
        var validation = httpContext.Request.Validate(request);
        if (!validation.IsValid)
        {
            return TypedResults.BadRequest(validation.Errors.First().ErrorMessage);
        }

        var userId = claimsPrincipal.Claims
            .SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        var existingDomain = await dbContext.Domains
            .SingleOrDefaultAsync(x => x.DomainName == request.DomainName && x.UserId == userId);

        if (existingDomain is not null)
        {
            return TypedResults.BadRequest($"Domain name {request.DomainName} already exists");
        }

        var verificationCode = dnsRecordsGenerator.GenerateDnsRecord(15);
        var domain = new Domain
        {
            DomainName = request.DomainName,
            UserId = userId ?? throw new InvalidOperationException("User id is null"),
            VerificationCode = verificationCode,
            VerificationDate = DateTime.UtcNow,
            IsVerified = false,
        };

        dbContext.Domains.Add(domain);
        await dbContext.SaveChangesAsync();

        var cnameInstructions = dnsRecordsGenerator.GetTxtInstructions(request.DomainName, verificationCode);
        var txtInstructions = dnsRecordsGenerator.GetCnameInstructions(verificationCode);

        return TypedResults.Created($"{httpContext.Request.Path}/status/{domain.DomainId}",
            new AddDomainNameResponse(txtInstructions, cnameInstructions));
    }


    public static async Task<Results<Ok<AddDomainNameResponse>, BadRequest<string>>>
        GetInstructionsHandler(
            HttpContext httpContext,
            ApplicationDbContext dbContext,
            ClaimsPrincipal claimsPrincipal,
            IDnsRecordsGenerator dnsRecordsGenerator,
            [AsParameters] AddDomainRequest request)
    {
        var validation = httpContext.Request.Validate(request);
        if (!validation.IsValid)
        {
            return TypedResults.BadRequest(validation.Errors.First().ErrorMessage);
        }

        var userId = claimsPrincipal.Claims
            .SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        var existingDomain = await dbContext.Domains
            .SingleOrDefaultAsync(x => x.DomainName == request.DomainName && x.UserId == userId);

        if (existingDomain is null)
        {
            return TypedResults.BadRequest($"Domain name {request.DomainName} does not exist");
        }

        var cnameInstructions =
            dnsRecordsGenerator.GetTxtInstructions(request.DomainName, existingDomain.VerificationCode);
        var txtInstructions = dnsRecordsGenerator.GetCnameInstructions(existingDomain.VerificationCode);
        
        return TypedResults.Ok(new AddDomainNameResponse(txtInstructions, cnameInstructions));
    }
}