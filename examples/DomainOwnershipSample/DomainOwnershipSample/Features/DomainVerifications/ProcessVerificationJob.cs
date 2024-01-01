using DomainOwnershipSample.Database.Context;
using DomainVerifier.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DomainOwnershipSample.Features.DomainVerifications;

[DisallowConcurrentExecution]
public class ProcessVerificationJob : IJob
{
    private readonly ILogger<ProcessVerificationJob> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IDnsRecordsVerifier _dnsRecordsVerifier;

    public ProcessVerificationJob(ILogger<ProcessVerificationJob> logger,
        ApplicationDbContext dbContext,
        IDnsRecordsVerifier dnsRecordsVerifier)
    {
        _logger = logger;
        _dbContext = dbContext;
        _dnsRecordsVerifier = dnsRecordsVerifier;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Loading 20 unverified domains");
        var unVerifiedDomains = await _dbContext.Domains
            .Where(x => !x.IsVerified && !_dbContext.Domains
                .Any(y => y.IsVerified && y.DomainName == x.DomainName))
            .OrderBy(x => x.VerificationDate)
            .Take(20)
            .ToListAsync(context.CancellationToken);
        _logger.LogInformation("Loaded {Count} unverified domains", unVerifiedDomains.Count);

        foreach (var domain in unVerifiedDomains)
        {
            _logger.LogInformation("Processing domain {DomainName}", domain.DomainName);

            domain.VerificationDate = DateTime.UtcNow;
            var isVerificationSucceeded =
                await _dnsRecordsVerifier.IsTxtRecordValidAsync(domain.DomainName, domain.VerificationCode)
                || await _dnsRecordsVerifier.IsCnameRecordValidAsync(domain.DomainName, domain.VerificationCode);

            if (!isVerificationSucceeded)
            {
                _logger.LogWarning("Domain {DomainName} is not verified", domain.DomainName);
            }
            else
            {
                _logger.LogInformation("Domain {DomainName} is verified", domain.DomainName);
                domain.IsVerified = true;
                domain.VerificationCompletedDate = DateTime.UtcNow;
            }
        }

        _dbContext.UpdateRange(unVerifiedDomains);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }
}