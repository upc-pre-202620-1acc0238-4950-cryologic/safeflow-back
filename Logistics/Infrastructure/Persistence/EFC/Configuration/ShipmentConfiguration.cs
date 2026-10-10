using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SafeFlow.API.Logistics.Domain.Model.Aggregates;

namespace SafeFlow.API.Logistics.Infrastructure.Persistence.EFC.Configuration;

public sealed class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(shipment => shipment.Id);
        builder.Property(shipment => shipment.Id).ValueGeneratedOnAdd();
        builder.HasIndex(shipment => shipment.ProductId);
        builder.HasIndex(shipment => shipment.LotId);
        builder.HasIndex(shipment => shipment.SensorId);
        builder.HasIndex(shipment => shipment.Status);
        builder.Property(shipment => shipment.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(shipment => shipment.Destination).HasColumnType("longtext").IsRequired();
        builder.Property(shipment => shipment.CreatedBy).HasColumnType("longtext").IsRequired();
        builder.Property(shipment => shipment.DepartedBy).HasColumnType("longtext");
        builder.Property(shipment => shipment.DeliveredBy).HasColumnType("longtext");

        // MySQL stores UTC datetime; conversion restores the UTC offset on materialization.
        var utc = new ValueConverter<DateTimeOffset, DateTime>(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
        builder.Property(shipment => shipment.CreatedAt).HasConversion(utc).HasColumnType("datetime(6)").IsRequired();
        builder.Property(shipment => shipment.DepartedAt).HasConversion(utc).HasColumnType("datetime(6)");
        builder.Property(shipment => shipment.DeliveredAt).HasConversion(utc).HasColumnType("datetime(6)");
    }
}
