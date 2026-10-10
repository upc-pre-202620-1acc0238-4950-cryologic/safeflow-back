using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;

namespace SafeFlow.API.Inventory.Infrastructure.Persistence.EFC.Configuration;

public sealed class LotConfiguration : IEntityTypeConfiguration<Lot>
{
    public void Configure(EntityTypeBuilder<Lot> builder)
    {
        builder.HasKey(lot => lot.Id);
        builder.Property(lot => lot.Id).ValueGeneratedOnAdd();
        builder.Property(lot => lot.Code).HasMaxLength(Lot.MaximumCodeLength)
            .UseCollation("utf8mb4_0900_as_ci").IsRequired();
        builder.Property(lot => lot.Version).IsConcurrencyToken().ValueGeneratedNever();
        builder.Ignore(lot => lot.Available);
        builder.HasIndex(lot => lot.ProductId);
        builder.HasIndex(lot => new { lot.ProductId, lot.Code }).IsUnique();
        builder.HasOne<Product>().WithMany().HasForeignKey(lot => lot.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
