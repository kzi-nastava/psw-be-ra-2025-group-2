using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Explorer.Encounters.Tests
{
    public class EncountersTestFactory : BaseTestFactory<EncountersContext>
    {
        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            ReplaceDbContext<EncountersContext>(services);
            ReplaceDbContext<PaymentsContext>(services); 

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
}
