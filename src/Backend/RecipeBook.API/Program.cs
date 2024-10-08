using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using RecipeBook.API.BackgroundServices;
using RecipeBook.API.Converters;
using RecipeBook.API.Filters;
using RecipeBook.API.Middleware;
using RecipeBook.API.Token;
using RecipeBook.Application;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Infrastructure;
using RecipeBook.Infrastructure.DataAccess;
using RecipeBook.Infrastructure.Extensions;
using RecipeBook.Infrastructure.Migrations;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(
    options => options.JsonSerializerOptions.Converters.Add(new StringConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<IdsFilter>();

    const string bearer = "Bearer";
    options.AddSecurityDefinition(bearer, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = bearer
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = bearer
                },
                Scheme = "oauth2",
                Name = bearer,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddHttpContextAccessor();

if (builder.Configuration.IsUnitTestEnviroment().IsFalse())
{
    builder.Services.AddHostedService<DeleteUserService>();
    AddGoogleAuthentication();
}

builder.Services.AddHealthChecks().AddDbContextCheck<RecipeBookDbContext>();

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

MigrateDatabase();

await app.RunAsync();

void MigrateDatabase()
{
    if (builder.Configuration.IsUnitTestEnviroment())
    {
        return;
    }

    var databaseType = builder.Configuration.DatabaseType();
    var connectionString = builder.Configuration.ConnectionString();

    DatabaseMigration.Migrate(databaseType, connectionString,
        app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
}

void AddGoogleAuthentication()
{
    var clientId = builder.Configuration.GetValue<string>("Settings:Google:ClientId")!;
    var clientSecret = builder.Configuration.GetValue<string>("Settings:Google:ClientSecret")!;

    builder.Services.AddAuthentication(config =>
        {
            config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
        });
}

public partial class Program
{
    protected Program()
    {
    }
}