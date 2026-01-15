using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.Payments.Tests
{
    public class PaymentsTestFactory : BaseTestFactory<PaymentsContext>
    {
        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
            services.Remove(descriptor!);
            services.AddDbContext<PaymentsContext>(SetupTestContext());


            //(minimalno DbContext od samog modula, kao i DbContext od svakog modula kog ovaj modul poziva (ako takvih ima)) ovako bi se to radilo
            //descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<OTHER_MODULEContext>));
            //services.Remove(descriptor!);
            //services.AddDbContext<OTHER_MODULEContext>(SetupTestContext());

            descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
            services.Remove(descriptor!);
            services.AddDbContext<ToursContext>(SetupTestContext());


            return services;
        }

    }
}
