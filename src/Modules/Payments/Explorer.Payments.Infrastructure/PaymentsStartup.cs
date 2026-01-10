using AutoMapper;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Mappers;
using Explorer.Payments.Core.UseCases;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database.Repositories;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Payments.Infrastructure
{
    public static class PaymentsStartup
    {
        public static IServiceCollection ConfigurePaymentsModule(this IServiceCollection services)
        {
           
            services.AddAutoMapper(typeof(PaymentsProfile).Assembly);
            SetupCore(services);
            SetupInfrastructure(services);

            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<IPurchaseService, PurchaseService>();

            services.AddScoped<IPaymentRecordService,PaymentRecordService>();

            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<INotificationService, NotificationService>();
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            // Primer:
            // services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenRepository>();

            services.AddScoped<IPaymentRecordRepository, PaymentRecordRepository>();

            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("payments"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<PaymentsContext>(options =>
                options.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));
        }
    }
}
