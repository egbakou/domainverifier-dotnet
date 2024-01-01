using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Carter.ModelBinding;
using DomainOwnershipSample.Database.Context;
using DomainVerifier.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DomainOwnershipSample.Features.DomainVerifications;

public record GetVerificationStatusRequest(string DomainName);

public class GetVerificationStatusRequestValidator : AbstractValidator<GetVerificationStatusRequest>
{
    public GetVerificationStatusRequestValidator()
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

public record VerificationStatusResponse(
    bool IsVerified,
    DateTime? VerificationDate,
    DateTime? VerificationCompletedDate);

public static class GetVerificationStatus
{
    public static async Task<Results<Ok<VerificationStatusResponse>, BadRequest<string>>>
        Handler(
            HttpContext httpContext,
            ApplicationDbContext dbContext,
            ClaimsPrincipal claimsPrincipal,
            IDnsRecordsVerifier dnsRecordsVerifier,
            [AsParameters] GetVerificationStatusRequest request
        )
    {
        var validation = httpContext.Request.Validate(request);
        if (!validation.IsValid)
        {
            return TypedResults.BadRequest(validation.Errors.First().ErrorMessage);
        }

        var userId = claimsPrincipal.Claims
            .SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        var domain = await dbContext.Domains
            .AsTracking()
            .SingleOrDefaultAsync(x => x.DomainName == request.DomainName && x.UserId == userId);

        if (domain is null)
        {
            return TypedResults.BadRequest($"Domain name {request.DomainName} not found");
        }

        var response = new VerificationStatusResponse(
            domain.IsVerified,
            domain.VerificationDate,
            domain.VerificationCompletedDate
        );
        return TypedResults.Ok(response);
    }
}