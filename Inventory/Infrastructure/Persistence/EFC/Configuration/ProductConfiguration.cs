using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;

namespace SafeFlow.API.Inventory.Infrastructure.Persistence.EFC.Configuration;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).ValueGeneratedOnAdd();
        builder.Property(product => product.Name).IsRequired();
        builder.Property(product => product.Minimum).IsRequired();
        builder.Property(product => product.Maximum).IsRequired();
    }
}
