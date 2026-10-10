using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Iam.Domain.Model.Aggregates;

namespace SafeFlow.API.Iam.Infrastructure.Persistence.EFC.Configuration;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.Permission).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.HasIndex(p => new { p.Role, p.Permission }).IsUnique();
    }
}
