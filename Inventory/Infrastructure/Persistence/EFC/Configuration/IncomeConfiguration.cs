using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;

namespace SafeFlow.API.Inventory.Infrastructure.Persistence.EFC.Configuration;

public sealed class IncomeConfiguration : IEntityTypeConfiguration<Income>
{
    public void Configure(EntityTypeBuilder<Income> builder)
    {
        builder.HasKey(income => income.Id);
        builder.Property(income => income.Id).ValueGeneratedOnAdd();
        builder.Property(income => income.Author).IsRequired();
        // MySQL datetime has no offset. Persist UTC and restore the zero offset explicitly.
        builder.Property(income => income.RecordedAt).HasConversion(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)))
            .HasColumnType("datetime(6)").IsRequired();
        builder.HasIndex(income => income.LotId);
        builder.HasIndex(income => income.ProductId);
        builder.HasOne<Lot>().WithMany().HasForeignKey(income => income.LotId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(income => income.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
