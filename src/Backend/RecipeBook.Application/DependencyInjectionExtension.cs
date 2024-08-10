using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Application.Services.AutoMapper;
using RecipeBook.Application.UseCases.Login;
using RecipeBook.Application.UseCases.Login.ExecuteLogin;
using RecipeBook.Application.UseCases.Recipe.Register;
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
        AddAutoMapper(serviceCollection, configuration);
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
    }

    private static void AddAutoMapper(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var sqids = new SqidsEncoder<long>(new SqidsOptions
        {
            MinLength = 10,
            Alphabet = configuration.GetValue<string>("Settings:CryptographyAlphabet")!
        });
        
        serviceCollection.AddScoped(provider => new AutoMapper.MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapping(sqids));
        }).CreateMapper());
    }
}