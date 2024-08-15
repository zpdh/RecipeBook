using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Application.Services.AutoMapper;
using RecipeBook.Application.UseCases.Dashboard.Get;
using RecipeBook.Application.UseCases.Login;
using RecipeBook.Application.UseCases.Login.ExecuteLogin;
using RecipeBook.Application.UseCases.Recipe.Delete;
using RecipeBook.Application.UseCases.Recipe.Filter;
using RecipeBook.Application.UseCases.Recipe.GetById;
using RecipeBook.Application.UseCases.Recipe.Register;
using RecipeBook.Application.UseCases.Recipe.Update;
using RecipeBook.Application.UseCases.User.ChangePassword;
using RecipeBook.Application.UseCases.User.Profile;
using RecipeBook.Application.UseCases.User.Registration;
using RecipeBook.Application.UseCases.User.Update;
using Sqids;

namespace RecipeBook.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        AddIdEncoder(serviceCollection, configuration);
        AddAutoMapper(serviceCollection);
        AddUseCases(serviceCollection);
    }

    private static void AddUseCases(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        serviceCollection.AddScoped<IExecuteLoginUseCase, ExecuteLoginUseCase>();
        serviceCollection.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
        serviceCollection.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        serviceCollection.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
        serviceCollection.AddScoped<IRegisterRecipeUseCase, RegisterRecipeUseCase>();
        serviceCollection.AddScoped<IFilterRecipeUseCase, FilterRecipeUseCase>();
        serviceCollection.AddScoped<IGetRecipeByIdUseCase, GetRecipeByIdUseCase>();
        serviceCollection.AddScoped<IDeleteRecipeUseCase, DeleteRecipeUseCase>();
        serviceCollection.AddScoped<IUpdateRecipeUseCase, UpdateRecipeUseCase>();

        serviceCollection.AddScoped<IGetDashboardUseCase, GetDashboardUseCase>();
    }

    private static void AddAutoMapper(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(provider => new AutoMapper.MapperConfiguration(options =>
        {
            var sqids = provider.GetService<SqidsEncoder<long>>()!;
            options.AddProfile(new AutoMapping(sqids));
        }).CreateMapper());
    }

    private static void AddIdEncoder(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var sqids = new SqidsEncoder<long>(new SqidsOptions
        {
            MinLength = 10,
            Alphabet = configuration.GetValue<string>("Settings:CryptographyAlphabet")!
        });

        serviceCollection.AddSingleton(sqids);
    }
}