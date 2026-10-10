using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using SafeFlow.API.Shared.Application.Seeding;

namespace SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Creates the database and schema if missing, then runs the context seeders. Only the test
/// accounts and their default permissions are seeded; operational lists start empty.
/// </summary>
public static class SafeFlowDbSeeder
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration, CancellationToken ct = default)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        await EnsureDatabaseExistsAsync(connectionString, ct);
        await services.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync(ct);
        foreach (var seeder in services.GetServices<IDataSeeder>())
            await seeder.SeedAsync(ct);
    }

    private static async Task EnsureDatabaseExistsAsync(string connectionString, CancellationToken ct)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        if (string.IsNullOrWhiteSpace(databaseName)) return;

        builder.Database = string.Empty;
        await using var connection = new MySqlConnection(builder.ConnectionString);
        await connection.OpenAsync(ct);
        await using var command = connection.CreateCommand();
        command.CommandText =
            $"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
        await command.ExecuteNonQueryAsync(ct);
    }
}
