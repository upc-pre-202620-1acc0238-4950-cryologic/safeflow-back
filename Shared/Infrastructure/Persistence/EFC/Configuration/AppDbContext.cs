using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Single unit of work shared by all bounded contexts. Each context maps its own entities through
/// IEntityTypeConfiguration classes under &lt;Context&gt;/Infrastructure/Persistence/EFC/Configuration.
/// Tables and columns follow the plural snake_case convention.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.UseSnakeCaseNamingConvention();
    }
}
