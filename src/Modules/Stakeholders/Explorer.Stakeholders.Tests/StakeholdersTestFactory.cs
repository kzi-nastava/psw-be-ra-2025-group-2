using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.API.Public.Emergency;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Explorer.Stakeholders.Tests.Integration;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        ReplaceDbContext<StakeholdersContext>(services);
        ReplaceDbContext<ToursContext>(services);
        ReplaceDbContext<PaymentsContext>(services);
        ReplaceDbContext<EncountersContext>(services);

        services.RemoveAll(typeof(IEmergencyPhrasebookProviderTransl));
        services.AddSingleton<IEmergencyPhrasebookProviderTransl, StubEmergencyPhrasebookProvider>();


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
