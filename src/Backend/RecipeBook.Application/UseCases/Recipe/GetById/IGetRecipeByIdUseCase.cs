using RecipeBook.Communication.Responses;

namespace RecipeBook.Application.UseCases.Recipe.GetById;

public interface IGetRecipeByIdUseCase
{
    Task<RecipeResponseJson> Execute(long recipeId);
}