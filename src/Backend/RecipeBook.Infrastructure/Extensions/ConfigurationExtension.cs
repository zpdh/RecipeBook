using Microsoft.Extensions.Configuration;
using RecipeBook.Domain.Enums;

namespace RecipeBook.Infrastructure.Extensions;

public static class ConfigurationExtension
{
    public static bool IsUnitTestEnviroment(this IConfiguration configuration)
    {
        return configuration.GetValue<bool>("InMemoryTest");
    }
    
    public static string ConnectionString(this IConfiguration configuration)
    {
        var databaseType = configuration.DatabaseType();

        return databaseType == Domain.Enums.DatabaseType.MySql
            ? configuration.GetConnectionString("MySqlConnection")!
            : configuration.GetConnectionString("SqlServerConnection")!;
    }

    public static DatabaseType DatabaseType(this IConfiguration configuration)
    {
        var databaseType = configuration.GetConnectionString("DatabaseType")!;

        return (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType);
    }
}