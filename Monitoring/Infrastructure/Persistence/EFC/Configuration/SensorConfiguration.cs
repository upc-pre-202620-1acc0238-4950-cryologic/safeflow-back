using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Monitoring.Domain.Model.Aggregates;

namespace SafeFlow.API.Monitoring.Infrastructure.Persistence.EFC.Configuration;

public sealed class SensorConfiguration : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> builder)
    {
        builder.HasKey(sensor => sensor.Id);
        builder.Property(sensor => sensor.Id).ValueGeneratedOnAdd();
        builder.Property(sensor => sensor.ProductId).IsRequired();
        builder.HasIndex(sensor => sensor.ProductId);
        builder.Property(sensor => sensor.Location).IsRequired().HasColumnType("text");
        builder.Property(sensor => sensor.Connected).IsRequired();
    }
}
