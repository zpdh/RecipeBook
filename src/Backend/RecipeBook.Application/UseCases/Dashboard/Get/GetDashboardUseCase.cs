using AutoMapper;
using RecipeBook.Application.Extensions;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Domain.Services.Storage;

namespace RecipeBook.Application.UseCases.Dashboard.Get;

public class GetDashboardUseCase : IGetDashboardUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IRecipeReadOnlyRepository _readOnlyRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetDashboardUseCase(
        ILoggedUser loggedUser,
        IMapper mapper,
        IRecipeReadOnlyRepository readOnlyRepository,
        IBlobStorageService blobStorageService)
    {
        _loggedUser = loggedUser;
        _mapper = mapper;
        _readOnlyRepository = readOnlyRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<RecipesResponseJson> Execute()
    {
        var user = await _loggedUser.User();

        var recipes = await _readOnlyRepository.GetDashboardRecipes(user);

        return new RecipesResponseJson
        {
            Recipes = await recipes.MapToShortRecipe(user, _mapper, _blobStorageService)
        };
    }
}