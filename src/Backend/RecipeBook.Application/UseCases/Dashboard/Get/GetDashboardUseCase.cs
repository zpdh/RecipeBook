using AutoMapper;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;

namespace RecipeBook.Application.UseCases.Dashboard.Get;

public class GetDashboardUseCase : IGetDashboardUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IRecipeReadOnlyRepository _readOnlyRepository;

    public GetDashboardUseCase(ILoggedUser loggedUser, IMapper mapper, IRecipeReadOnlyRepository readOnlyRepository)
    {
        _loggedUser = loggedUser;
        _mapper = mapper;
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<RecipesResponseJson> Execute()
    {
        var user = await _loggedUser.User();

        var recipes = await _readOnlyRepository.GetDashboardRecipes(user);

        return new RecipesResponseJson
        {
            Recipes = _mapper.Map<IList<ShortRecipeResponseJson>>(recipes)
        };
    }
}