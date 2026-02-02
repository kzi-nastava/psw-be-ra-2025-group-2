using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Encounters.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        ReplaceDbContext<StakeholdersContext>(services);
        ReplaceDbContext<ToursContext>(services);
        ReplaceDbContext<PaymentsContext>(services);
        ReplaceDbContext<EncountersContext>(services);

        return services;
    }

    private void ReplaceDbContext<TContext>(IServiceCollection services)
        where TContext : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TContext>));
        if (descriptor != null) services.Remove(descriptor);

        services.AddDbContext<TContext>(SetupTestContext());
    }
}
