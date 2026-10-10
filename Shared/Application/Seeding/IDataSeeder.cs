namespace SafeFlow.API.Shared.Application.Seeding;

/// <summary>Idempotent startup seed owned by a bounded context. Never seeds operational data.</summary>
public interface IDataSeeder
{
    Task SeedAsync(CancellationToken ct = default);
}
