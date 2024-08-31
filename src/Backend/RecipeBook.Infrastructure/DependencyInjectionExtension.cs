using System.Reflection;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI_API;
using RecipeBook.Domain.Enums;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Security.Cryptography;
using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Domain.Services.OpenAI;
using RecipeBook.Domain.Services.ServiceBus;
using RecipeBook.Domain.Services.Storage;
using RecipeBook.Infrastructure.DataAccess;
using RecipeBook.Infrastructure.DataAccess.Repositories;
using RecipeBook.Infrastructure.Extensions;
using RecipeBook.Infrastructure.Security.Cryptography;
using RecipeBook.Infrastructure.Security.Tokens.Generators;
using RecipeBook.Infrastructure.Security.Tokens.Validators;
using RecipeBook.Infrastructure.Services.LoggedUser;
using RecipeBook.Infrastructure.Services.OpenAI;
using RecipeBook.Infrastructure.Services.ServiceBus;
using RecipeBook.Infrastructure.Services.Storage;

namespace RecipeBook.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        AddRepositories(serviceCollection);
        AddLoggedUser(serviceCollection);
        AddTokens(serviceCollection, configuration);
        AddPasswordEncrypter(serviceCollection);
        AddOpenAI(serviceCollection, configuration);
        AddAzureStorage(serviceCollection, configuration);
        AddQueue(serviceCollection, configuration);

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
        serviceCollection.AddScoped<IUserDeleteOnlyRepository, UserRepository>();

        serviceCollection.AddScoped<IRecipeWriteOnlyRepository, RecipeRepository>();
        serviceCollection.AddScoped<IRecipeReadOnlyRepository, RecipeRepository>();
        serviceCollection.AddScoped<IRecipeUpdateOnlyRepository, RecipeRepository>();

        serviceCollection.AddScoped<ITokenRepository, TokenRepository>();
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

        serviceCollection.AddScoped<IRefreshTokenGenerator>(_ => new RefreshTokenGenerator());
    }

    private static void AddLoggedUser(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ILoggedUser, LoggedUser>();
    }

    private static void AddOpenAI(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IGenerateRecipeAI, ChatGptService>();

        var key = configuration.GetValue<string>("Settings:OpenAI:ApiKey");
        var authentication = new APIAuthentication(key);

        services.AddScoped<IOpenAIAPI>(option => new OpenAIAPI(authentication));
    }

    private static void AddPasswordEncrypter(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IPasswordEncrypter, BCryptHasher>();
    }

    private static void AddAzureStorage(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Settings:BlobStorage:Azure");

        // Checking if empty in order for tests to not
        // throw exceptions  the connection string
        if (connectionString.IsNotEmpty())
        {
            serviceCollection.AddScoped<IBlobStorageService>(_ =>
                new AzureStorageService(
                    new BlobServiceClient(connectionString)));
        }
    }

    private static void AddQueue(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Settings:ServiceBus:DeleteAccount");
        const string queueName = "user";

        if (string.IsNullOrWhiteSpace(connectionString)) return;


        var client = new ServiceBusClient(
            connectionString,
            new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

        var queue = new DeleteUserQueue(client.CreateSender(queueName));
        var processor = new DeleteUserProcessor(
            client.CreateProcessor(
                queueName,
                new ServiceBusProcessorOptions
                {
                    MaxConcurrentCalls = 1
                }));

        serviceCollection.AddSingleton(processor);
        serviceCollection.AddScoped<IDeleteUserQueue>(_ => queue);
    }
}