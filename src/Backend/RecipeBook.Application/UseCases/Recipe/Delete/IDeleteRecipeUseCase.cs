namespace RecipeBook.Application.UseCases.Recipe.Delete;

public interface IDeleteRecipeUseCase
{
    public Task Execute(long id);
}