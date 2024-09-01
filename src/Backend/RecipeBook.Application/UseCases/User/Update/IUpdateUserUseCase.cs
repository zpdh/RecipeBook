using RecipeBook.Communication.Requests;

namespace RecipeBook.Application.UseCases.User.Update;

public interface IUpdateUserUseCase
{
    public Task Execute(UpdateUserRequestJson request);
}