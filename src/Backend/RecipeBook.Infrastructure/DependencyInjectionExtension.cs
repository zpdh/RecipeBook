using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Domain.Enums;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Infrastructure.DataAccess;
using RecipeBook.Infrastructure.DataAccess.Repositories;

namespace RecipeBook.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var databaseType = configuration.GetConnectionString("DatabaseType")!;

        var typeEnum = (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType);

        if (typeEnum == DatabaseType.MySql) AddDbContextMySql(serviceCollection, configuration);
        else AddDbContextSqlServer(serviceCollection, configuration);

        AddRepositories(serviceCollection);
    }

    private static void AddRepositories(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

        serviceCollection.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        serviceCollection.AddScoped<IUserReadOnlyRepository, UserRepository>();
    }

    private static void AddDbContextSqlServer(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<RecipeBookDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));
    }

    private static void AddDbContextMySql(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 38));

        serviceCollection.AddDbContext<RecipeBookDbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("MySqlConnection"), serverVersion));
    }
}