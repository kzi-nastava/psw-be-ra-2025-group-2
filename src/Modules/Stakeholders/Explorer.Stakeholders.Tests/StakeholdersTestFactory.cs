using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(descriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        services.Remove(descriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
        services.Remove(descriptor!);
        services.AddDbContext<PaymentsContext>(SetupTestContext());

        return services;
    }
}