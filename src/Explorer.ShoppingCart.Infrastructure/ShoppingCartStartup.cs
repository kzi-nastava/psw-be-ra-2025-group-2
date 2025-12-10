using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.ShoppingCart.Core.Interfaces;
using Explorer.ShoppingCart.Core.Mappers; 
using Explorer.ShoppingCart.Core.UseCases.Shopping;
using Explorer.ShoppingCart.Infrastructure.Repositories;
using Explorer.Tours.Core.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.ShoppingCart.Infrastructure;

public static class ShoppingCartStartup
{
    public static IServiceCollection ConfigureShoppingCartModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ShoppingCartProfile).Assembly);
        services.AddAutoMapper(typeof(PurchaseProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenRepository>();
    }
}