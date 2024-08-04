using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.Application.UseCases.Login.ExecuteLogin;

public interface IExecuteLoginUseCase
{
    public Task<RegisterUserResponseJson> Execute(LoginRequestJson request);
}