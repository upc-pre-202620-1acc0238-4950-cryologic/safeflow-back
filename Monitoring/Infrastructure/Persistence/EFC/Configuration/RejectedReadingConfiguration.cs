using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeFlow.API.Monitoring.Domain.Model.Aggregates;

namespace SafeFlow.API.Monitoring.Infrastructure.Persistence.EFC.Configuration;

public sealed class RejectedReadingConfiguration : IEntityTypeConfiguration<RejectedReading>
{
    public void Configure(EntityTypeBuilder<RejectedReading> builder)
    {
        builder.HasKey(reading => reading.Id);
        builder.Property(reading => reading.Id).ValueGeneratedOnAdd();
        builder.Property(reading => reading.SensorId).IsRequired();
        builder.HasOne<Sensor>().WithMany().HasForeignKey(reading => reading.SensorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(reading => reading.SensorId);
        builder.Property(reading => reading.RecordedAt).IsRequired().HasColumnType("datetime(6)")
            .HasConversion(value => value.UtcDateTime,
                value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
        builder.Property(reading => reading.Raw).IsRequired().HasColumnType("longtext");
        builder.Property(reading => reading.Reason).IsRequired().HasColumnType("text");
    }
}
