using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Iam.Domain.Model.Aggregates;

namespace SafeFlow.API.Iam.Infrastructure.Persistence.EFC.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();
        builder.Property(u => u.Email).HasMaxLength(320).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(u => u.Failures).IsConcurrencyToken();
        builder.Property(u => u.BlockedUntil).HasConversion<UtcDateTimeOffsetConverter>()
            .HasColumnType("datetime(6)").IsConcurrencyToken();
        builder.Property(u => u.VerificationRequestedAt).HasConversion<UtcDateTimeOffsetConverter>().HasColumnType("datetime(6)");
        builder.Property(u => u.RecoveryRequestedAt).HasConversion<UtcDateTimeOffsetConverter>().HasColumnType("datetime(6)");
    }
}
