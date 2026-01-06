using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        services.ConfigureToursModule();

        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<ToursContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(descriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        var internalUserServiceMock = new Mock<IInternalUserService>();

        services.AddSingleton(internalUserServiceMock);
        services.AddSingleton(internalUserServiceMock.Object);

        return services;
    }
}