using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Public.Administration;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Mappers;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Core.UseCases.Administration;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Stakeholders.Infrastructure;

public static class StakeholdersStartup
{
    public static IServiceCollection ConfigureStakeholdersModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StakeholderProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }
    
    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IAuthorAwardsService, AuthorAwardsService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITokenGenerator, JwtGenerator>();

        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<ITourPreferencesService, TourPreferencesService>();
        services.AddScoped<IClubService, ClubService>();
        services.AddScoped<IAppRatingService, AppRatingService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IDiaryService, DiaryService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonDbRepository>();
        services.AddScoped<IUserRepository, UserDbRepository>();
        services.AddScoped<ITourPreferencesRepository, TourPreferencesDbRepository>();
        services.AddScoped<IAuthorAwardsRepository, AuthorAwardsDbRepository>();
        services.AddScoped<IAppRatingRepository, AppRatingRepository>();
        services.AddScoped<IClubRepository, ClubDbRepository>();
        services.AddScoped<IDiaryRepository, DiaryDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("stakeholders"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        
        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));
    }
}
