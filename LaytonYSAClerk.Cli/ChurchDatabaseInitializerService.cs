using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace LaytonYSAClerk.Cli;

public class ChurchDatabaseInitializerService : BackgroundService 
{
    private readonly ILogger<ChurchDatabaseInitializerService> _logger;
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _lifetime;
    
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource trace = new(ActivitySourceName);
    public ChurchDatabaseInitializerService(
        ILogger<ChurchDatabaseInitializerService> logger,
        IServiceProvider services,
        IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _services = services;
        _lifetime = lifetime;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var span = trace.StartActivity("Migrating database", ActivityKind.Client);
        try
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChurchDbContext>();

            await EnsureDatabaseAsync(dbContext, cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            span?.RecordException(ex);
            throw;
        }
    }
    
    private static async Task EnsureDatabaseAsync(ChurchDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private static async Task RunMigrationAsync(ChurchDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}