using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Mappers;
using Explorer.Blog.Core.UseCases;
using Explorer.Blog.Infrastructure.Database;
using Explorer.Blog.Infrastructure.Database.Repositories;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Explorer.Blog.Infrastructure;

public static class BlogStartup
{
    public static IServiceCollection ConfigureBlogModule(this IServiceCollection services)
    {
        // Registers all profiles since it works on the assembly
        services.AddAutoMapper(typeof(BlogProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IBlogPostService, BlogPostService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("blog"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<BlogContext>(opt =>
        {
            // Tvoja postoje?a konfiguracija za bazu
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "blog"));

            // --- NOVO: OVO SPRE?AVA PUCANJE ---

            // 1. Isklju?uje logovanje osetljivih podataka (vrednosti parametara)
            opt.EnableSensitiveDataLogging(false);

            // 2. Potpuno ignoriše ispisivanje SQL komandi u konzolu
            // Ovo spašava Visual Studio od gušenja Base64 stringom
            opt.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.CommandExecuted));

            // ----------------------------------
        });

        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
    }
}
