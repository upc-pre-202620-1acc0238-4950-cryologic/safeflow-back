using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Iam.Domain.Model.Aggregates;

namespace SafeFlow.API.Iam.Infrastructure.Persistence.EFC.Configuration;

public sealed class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.Jti).HasMaxLength(64).IsRequired();
        builder.HasIndex(t => t.Jti).IsUnique();
        builder.Property(t => t.ExpiresAt).HasConversion<UtcDateTimeOffsetConverter>().HasColumnType("datetime(6)");
    }
}
