using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Monitoring.Domain.Model.Aggregates;

namespace SafeFlow.API.Monitoring.Infrastructure.Persistence.EFC.Configuration;

public sealed class ReadingConfiguration : IEntityTypeConfiguration<Reading>
{
    public void Configure(EntityTypeBuilder<Reading> builder)
    {
        builder.HasKey(reading => reading.Id);
        builder.Property(reading => reading.Id).ValueGeneratedOnAdd();
        builder.Property(reading => reading.SensorId).IsRequired();
        builder.HasOne<Sensor>().WithMany().HasForeignKey(reading => reading.SensorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(reading => reading.SensorId);
        builder.HasIndex(reading => new { reading.SensorId, reading.RecordedAt });
        builder.Property(reading => reading.ShipmentId).IsRequired(false);
        builder.HasIndex(reading => reading.ShipmentId);
        builder.Property(reading => reading.RecordedAt).IsRequired().HasColumnType("datetime(6)")
            .HasConversion(value => value.UtcDateTime,
                value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
        builder.Property(reading => reading.Temperature).IsRequired();
        builder.Property(reading => reading.Condition).IsRequired().HasConversion<string>().HasMaxLength(16);
    }
}
