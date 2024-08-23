using CommonTestUtils.Blob;
using CommonTestUtils.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Domain.Services.Storage;
using RecipeBook.Infrastructure.DataAccess;

namespace Api.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public RecipeBook.Domain.Entities.User User { get; private set; } = default!;
    public string Password { get; private set; } = string.Empty;
    public RecipeBook.Domain.Entities.Recipe Recipe { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test").ConfigureServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<RecipeBookDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            var blobStorage = new BlobStorageServiceBuilder().Build();

            // Adding a blob storage mock to provide an 
            // instance for the integration tests
            services.AddScoped<IBlobStorageService>(_ => blobStorage);

            services.AddDbContext<RecipeBookDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
                options.UseInternalServiceProvider(provider);
            });

            using var scope = services.BuildServiceProvider().CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<RecipeBookDbContext>();

            context.Database.EnsureDeleted();

            StartDatabase(context);
        });
    }


    private void StartDatabase(RecipeBookDbContext context)
    {
        (User, Password) = UserBuilder.Build();

        Recipe = RecipeBuilder.Build(User);

        context.Users.Add(User);
        context.Recipes.Add(Recipe);

        context.SaveChanges();
    }
}