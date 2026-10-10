using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SafeFlow.API.Iam.Infrastructure.Persistence.EFC.Configuration;

public sealed class UtcDateTimeOffsetConverter() : ValueConverter<DateTimeOffset, DateTime>(
    value => value.UtcDateTime,
    value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
