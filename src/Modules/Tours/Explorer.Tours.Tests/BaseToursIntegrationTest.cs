using Explorer.BuildingBlocks.Tests;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Tests;

public class BaseToursIntegrationTest : BaseWebIntegrationTest<ToursTestFactory>
{
    public BaseToursIntegrationTest(ToursTestFactory factory) : base(factory)
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // SQL fajlovi - izvršavaj u pravom redosledu
        var sqlFiles = new[]
        {
        Path.Combine(AppContext.BaseDirectory, "TestData/a-delete.sql"),   // prvo očisti
        Path.Combine(AppContext.BaseDirectory, "TestData/b-users.sql"),    // zatim korisnici
        Path.Combine(AppContext.BaseDirectory, "TestData/c-tours.sql"),    // ture
        Path.Combine(AppContext.BaseDirectory, "TestData/b-equipment.sql") // oprema
    };

        foreach (var file in sqlFiles)
        {
            if (File.Exists(file))
            {
                var sql = File.ReadAllText(file);
                dbContext.Database.ExecuteSqlRaw(sql);
            }
            else
            {
                throw new FileNotFoundException($"SQL file not found: {file}");
            }
        }
    }
}