using DomainOwnershipSample.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainOwnershipSample.Database.Configurations;

public class DomainVerificationConfiguration : IEntityTypeConfiguration<Domain>
{
    public void Configure(EntityTypeBuilder<Domain> builder)
    {
        builder.HasKey(x => x.DomainId);
        builder.Property(x => x.DomainId).ValueGeneratedOnAdd();

        builder.Property(x => x.DomainName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.VerificationCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.VerificationDate).IsRequired();
        builder.Property(x => x.IsVerified).IsRequired();
        builder.Property(x => x.VerificationCompletedDate).IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Domains)
            .HasForeignKey(x => x.UserId);
    }
}