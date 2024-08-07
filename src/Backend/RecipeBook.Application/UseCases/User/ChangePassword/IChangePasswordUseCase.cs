using RecipeBook.Communication.Requests;

namespace RecipeBook.Application.UseCases.User.ChangePassword;

public interface IChangePasswordUseCase
{
    public Task Execute(ChangePasswordRequestJson request);
}