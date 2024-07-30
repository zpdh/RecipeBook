using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.Application.UseCases.User.Registration;

public interface IRegisterUserUseCase
{
    public Task<RegisterUserResponseJson> Execute(RegisterUserRequestJson request);
}