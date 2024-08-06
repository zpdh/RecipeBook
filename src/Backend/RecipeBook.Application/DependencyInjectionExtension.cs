using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Application.Services.AutoMapper;
using RecipeBook.Application.Services.Cryptography;
using RecipeBook.Application.UseCases.Login;
using RecipeBook.Application.UseCases.Login.ExecuteLogin;
using RecipeBook.Application.UseCases.User.Profile;
using RecipeBook.Application.UseCases.User.Registration;

namespace RecipeBook.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        AddPasswordEncrypter(serviceCollection, configuration);
        AddAutoMapper(serviceCollection);
        AddUseCases(serviceCollection);
    }

    private static void AddUseCases(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        serviceCollection.AddScoped<IExecuteLoginUseCase, ExecuteLoginUseCase>();
        serviceCollection.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
    }

    private static void AddAutoMapper(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(provider => new AutoMapper.MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapping());
        }).CreateMapper());
    }

    private static void AddPasswordEncrypter(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var passKey = configuration.GetValue<string>("Settings:Password:PasswordKey")!;
        
        serviceCollection.AddScoped(options => new PasswordEncrypter(passKey));
    }
}