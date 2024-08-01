using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using RecipeBook.Domain.Enums;

namespace RecipeBook.Infrastructure.Migrations;

public class DatabaseMigration
{
    public static void Migrate(DatabaseType databaseType, string connectionString, IServiceProvider serviceProvider)
    {
        if (databaseType == DatabaseType.MySql) EnsureDatabaseCreationMySql(connectionString);
        else EnsureDatabaseCreationSqlServer(connectionString);
        
        MigrateDatabase(serviceProvider);
    }

    private static void EnsureDatabaseCreationMySql(string connectionString)
    {
        var csBuilder = new MySqlConnectionStringBuilder(connectionString);

        var databaseName = csBuilder.Database;
        csBuilder.Remove("Database");

        using var dbConnection = new MySqlConnection(csBuilder.ConnectionString);

        var parameters = new DynamicParameters();
        parameters.Add("dbname", databaseName);

        var records = dbConnection.Query(sql:
            "SELECT * FROM INFORMATION_SCHEMA.SCHEMATA " +
            "WHERE SCHEMA_NAME = @dbname", parameters);

        if (!records.Any()) dbConnection.Execute(sql: $"CREATE DATABASE {databaseName}");
    }

    private static void EnsureDatabaseCreationSqlServer(string connectionString)
    {
        var csBuilder = new SqlConnectionStringBuilder(connectionString);

        var databaseName = csBuilder.InitialCatalog;
        csBuilder.Remove("Initial Catalog");

        using var dbConnection = new SqlConnection(csBuilder.ConnectionString);

        var parameters = new DynamicParameters();
        parameters.Add("dbname", databaseName);

        var records = dbConnection.Query(sql:
            "SELECT * FROM sys.databases " +
            "WHERE NAME = @dbname", parameters);

        if (!records.Any()) dbConnection.Execute(sql: $"CREATE DATABASE {databaseName}");
    }

    private static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        
        runner.ListMigrations();
        runner.MigrateUp();
    }
}