using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.Application.UseCases.Recipe.Register;

public interface IRegisterRecipeUseCase
{
    public Task<RegisteredRecipeResponseJson> Execute(RecipeRequestJson request);
}