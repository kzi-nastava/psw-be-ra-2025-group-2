using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Explorer.API;

namespace Explorer.BuildingBlocks.Tests;

public abstract class BaseTestFactory<TDbContext> : WebApplicationFactory<Program> where TDbContext : DbContext
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using var scope = BuildServiceProvider(services).CreateScope();
            var scopedServices = scope.ServiceProvider;  
            var db = scopedServices.GetRequiredService<TDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<BaseTestFactory<TDbContext>>>();

            var path = Path.Combine(".", "..", "..", "..", "TestData");


            InitializeDatabase(db, path, logger);
        });
    }

    private static void InitializeDatabase(DbContext context, string scriptFolder, ILogger logger)
    {
        // 1) Uvek obriši i ponovo kreiraj testnu bazu
        try
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();


            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch (Exception)
        {
            // CreateTables baca exception ako tabela već postoji - ignoriši
        }

        // 2) Pronađi sve SQL fajlove u TestData folderu
        var scriptFiles = Directory.GetFiles(scriptFolder);
        Array.Sort(scriptFiles);

        // 3) Loguj koji se fajlovi izvršavaju
        Console.WriteLine("=== SQL FILES FOUND IN TESTDATA ===");
        foreach (var f in scriptFiles)
        {
            Console.WriteLine("EXECUTING SQL FILE: " + f);
        }

        // 4) Izvrši SQL skripte i ulovi sve SQL greške
        try
        {
            var script = string.Join("\n", scriptFiles.Select(File.ReadAllText));
            context.Database.ExecuteSqlRaw(script);
        }
        catch (Exception ex)
        {
            Console.WriteLine("SQL ERROR: " + ex.Message);
            logger.LogError(ex, "Error executing SQL test data scripts.");
            throw;       // bacamo exception da ZAISTA vidiš grešku iz SQL-a
        }
    }


    private ServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        return ReplaceNeededDbContexts(services).BuildServiceProvider();
    }

    protected abstract IServiceCollection ReplaceNeededDbContexts(IServiceCollection services);

    protected static Action<DbContextOptionsBuilder> SetupTestContext()
    {
        var server = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? "explorer-v1-test";
        var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "root";
        var pooling = Environment.GetEnvironmentVariable("DATABASE_POOLING") ?? "true";

        var connectionString = $"Server={server};Port={port};Database={database};User ID={user};Password={password};Pooling={pooling};Include Error Detail=True";

        return opt => opt.UseNpgsql(connectionString);
    }
}