using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Domain.Enums;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Infrastructure.DataAccess;
using RecipeBook.Infrastructure.DataAccess.Repositories;
using RecipeBook.Infrastructure.Extensions;
using RecipeBook.Infrastructure.Security.Tokens.Generators;
using RecipeBook.Infrastructure.Security.Tokens.Validators;
using RecipeBook.Infrastructure.Services.LoggedUser;

namespace RecipeBook.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        AddRepositories(serviceCollection);
        AddLoggedUser(serviceCollection);
        AddTokens(serviceCollection, configuration);

        if (configuration.IsUnitTestEnviroment())
        {
            return;
        }

        var databaseType = configuration.DatabaseType();

        if (databaseType == DatabaseType.MySql)
        {
            AddDbContextMySql(serviceCollection, configuration);
            AddFluentMigratorMySql(serviceCollection, configuration);
        }
        else
        {
            AddDbContextSqlServer(serviceCollection, configuration);
            AddFluentMigratorSqlServer(serviceCollection, configuration);
        }
    }

    private static void AddRepositories(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

        serviceCollection.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        serviceCollection.AddScoped<IUserReadOnlyRepository, UserRepository>();
        serviceCollection.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
    }

    private static void AddDbContextSqlServer(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<RecipeBookDbContext>(options =>
            options.UseSqlServer(configuration.ConnectionString()));
    }

    private static void AddDbContextMySql(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 38));

        serviceCollection.AddDbContext<RecipeBookDbContext>(options =>
            options.UseMySql(configuration.ConnectionString(), serverVersion));
    }

    private static void AddFluentMigratorMySql(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddFluentMigratorCore().ConfigureRunner(options =>
        {
            options.AddMySql5().WithGlobalConnectionString(configuration.ConnectionString())
                .ScanIn(Assembly.Load("RecipeBook.Infrastructure")).For.All();
        });
    }

    private static void AddFluentMigratorSqlServer(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddFluentMigratorCore().ConfigureRunner(options =>
        {
            options.AddSqlServer().WithGlobalConnectionString(configuration.ConnectionString())
                .ScanIn(Assembly.Load("RecipeBook.Infrastructure")).For.All();
        });
    }

    private static void AddTokens(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var expirationInMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationInMinutes");
        var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

        serviceCollection.AddScoped<IAccessTokenGenerator>(_ =>
            new JwtTokenGenerator(expirationInMinutes, signingKey!));
        serviceCollection.AddScoped<IAccessTokenValidator>(_ => new JwtTokenValidator(signingKey!));
    }

    private static void AddLoggedUser(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ILoggedUser, LoggedUser>();
    }
}