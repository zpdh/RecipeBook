namespace RecipeBook.Application.UseCases.Login.ExternalLogin;

public interface IExternalLoginUseCase
{
    Task<string> Execute(string name, string email);
}