using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Public.Administration;
using Explorer.Stakeholders.Core.Domain.Planner;
using Explorer.Stakeholders.Core.Domain.Planner.Services;
using Explorer.Stakeholders.API.Public.Emergency;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Mappers;
using Explorer.Stakeholders.Core.Services;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Core.UseCases.Administration;
using Explorer.Stakeholders.Core.UseCases.Emergency;
using Explorer.Stakeholders.Core.UseCases.Internal;
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
        services.AddScoped<IMeetupService, MeetupService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<ITourPreferencesService, TourPreferencesService>();
        services.AddScoped<IClubService, ClubService>();
        services.AddScoped<IClubLeaderboardService, ClubLeaderboardService>();
        services.AddScoped<IAppRatingService, AppRatingService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDiaryService, DiaryService>();
        services.AddScoped<IPeopleNameProvider, PeopleNameProvider>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPlannerService, PlannerService>();
        services.AddScoped<IClubBadgeService, ClubBadgeService>();
        services.AddScoped<IQuizService, QuizService>();

        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IClubChatService, ClubChatService>();
        services.AddScoped<IOnboardingService, OnboardingService>();
        services.AddScoped<IUsernameProvider, UsernameProviderService>();
        services.AddScoped<ITouristPositionService, TouristPositionService>();
        
        services.AddScoped<IFaqService, FaqService>();
        services.AddScoped<IHelpSettingsService, HelpSettingsService>();

        services.AddScoped<IPlanEvaluator, PlanEvaluator>();

        /* Internal */
        services.AddScoped<IInternalUserService, InternalUserService>();

       
        services.AddScoped<IEmergencyOverviewService, EmergencyOverviewService>();

    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonDbRepository>();
        services.AddScoped<IUserRepository, UserDbRepository>();
        services.AddScoped<IMeetupRepository, MeetupDbRepository>();
        services.AddScoped<ITourPreferencesRepository, TourPreferencesDbRepository>();
        services.AddScoped<IAuthorAwardsRepository, AuthorAwardsDbRepository>();
        services.AddScoped<IAppRatingRepository, AppRatingRepository>();
        services.AddScoped<IClubRepository, ClubDbRepository>();

        services.AddScoped<IDiaryRepository, DiaryDbRepository>();      


        services.AddScoped<IQuizRepository, QuizDbRepository>();

        services.AddScoped<IMessageRepository, MessageDbRepository>();
        services.AddScoped<IChatRepository, ChatDbRepository>();
        services.AddScoped<IOnboardingRepository, OnboardingDbRepository>();
        services.AddScoped<ITouristPositionRepository, TouristPositionDbRepository>();

        services.AddScoped<IFaqRepository, FaqRepository>();
        services.AddScoped<IHelpSettingsRepository, HelpSettingsRepository>();
        services.AddScoped<IPlannerRepository, PlannerDbRepository>();

        services.AddScoped<IEmergencyDirectoryRepository, EmergencyDirectoryRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("stakeholders"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));
    }
}