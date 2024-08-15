using RecipeBook.Communication.Responses;

namespace RecipeBook.Application.UseCases.Dashboard.Get;

public interface IGetDashboardUseCase
{
    public Task<RecipesResponseJson> Execute();
}