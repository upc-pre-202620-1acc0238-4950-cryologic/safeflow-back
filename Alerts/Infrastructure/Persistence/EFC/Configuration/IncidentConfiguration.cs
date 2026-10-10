using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SafeFlow.API.Alerts.Domain.Model.Aggregates;

namespace SafeFlow.API.Alerts.Infrastructure.Persistence.EFC.Configuration;

public sealed class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.HasKey(incident => incident.Id);
        builder.Property(incident => incident.Id).ValueGeneratedOnAdd();
        builder.Property(incident => incident.SensorId).IsRequired();
        builder.Property(incident => incident.ProductId).IsRequired();
        builder.HasIndex(incident => incident.SensorId);
        builder.HasIndex(incident => incident.ProductId);
        builder.HasIndex(incident => incident.ResolvedAt);

        builder.Property(incident => incident.Location).IsRequired().HasColumnType("longtext");
        builder.Property(incident => incident.Type).IsRequired().HasColumnType("longtext");
        builder.Property(incident => incident.Description).IsRequired().HasColumnType("longtext");
        builder.Property(incident => incident.Severity).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(incident => incident.Repetitions).IsRequired();
        builder.Property(incident => incident.Action).HasColumnType("longtext");
        builder.Property(incident => incident.Author).HasColumnType("longtext");

        var utc = new ValueConverter<DateTimeOffset, DateTime>(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
        builder.Property(incident => incident.OpenedAt).HasConversion(utc).HasColumnType("datetime(6)").IsRequired();
        builder.Property(incident => incident.LastSeenAt).HasConversion(utc).HasColumnType("datetime(6)").IsRequired();
        // The original NULL participates in UPDATE, so concurrent resolutions cannot overwrite history.
        builder.Property(incident => incident.ResolvedAt).HasConversion(utc).HasColumnType("datetime(6)")
            .IsConcurrencyToken();
        builder.Ignore(incident => incident.Active);
    }
}
