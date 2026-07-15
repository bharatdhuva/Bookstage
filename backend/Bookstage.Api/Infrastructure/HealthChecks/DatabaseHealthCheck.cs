using Bookstage.Api.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bookstage.Api.Infrastructure.HealthChecks;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly BookstageDbContext _context;

    public DatabaseHealthCheck(BookstageDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return await _context.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Database connection failed");
    }
}