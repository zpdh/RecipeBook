using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.Application.UseCases.Recipe.Filter;

public interface IFilterRecipeUseCase
{
    public Task<RecipesResponseJson> Execute(RecipeFilterRequestJson request);
}