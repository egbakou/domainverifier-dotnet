using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DomainOwnershipSample.Features.DomainVerifications;

public class DomainVerificationsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/domains")
            .RequireAuthorization()
            .WithOpenApi()
            .WithTags("domains");
        
        group.MapPost("", AddDomain.Handler)
            .WithName(nameof(AddDomain))
            .WithSummary("Registers a domain name for ownership verification");
        
        group.MapGet("instructions", AddDomain.GetInstructionsHandler)
            .WithName("GetInstructions")
            .WithSummary("Retrieve detailed instructions for adding DNS records to the DNS settings for domain verification");

        group.MapGet("status", GetVerificationStatus.Handler)
            .WithName(nameof(GetVerificationStatus))
            .WithSummary("Retrieve the current verification status of a specified domain name");
    }
}