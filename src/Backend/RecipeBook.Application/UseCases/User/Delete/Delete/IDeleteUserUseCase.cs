namespace RecipeBook.Application.UseCases.User.Delete.Delete;

public interface IDeleteUserUseCase
{
    Task Execute(Guid userIdentifier);
}