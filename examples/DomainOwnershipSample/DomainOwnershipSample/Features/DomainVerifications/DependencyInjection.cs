using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DomainOwnershipSample.Features.DomainVerifications;

public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(ProcessVerificationJob));
            q.AddJob<ProcessVerificationJob>(opts => opts.WithIdentity(jobKey));
            
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{nameof(ProcessVerificationJob)}-trigger")
                .WithCronSchedule("0 0/5 * * * ? *") // Every 5 minutes
            );
        });
        
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
}