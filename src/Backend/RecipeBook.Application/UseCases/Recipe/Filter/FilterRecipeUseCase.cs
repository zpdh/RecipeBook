using AutoMapper;
using RecipeBook.Application.Extensions;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.DTOs;
using RecipeBook.Domain.Enums;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Domain.Services.Storage;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipeUseCase : IFilterRecipeUseCase
{
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _readOnlyRepository;
    private readonly IBlobStorageService _blobStorageService;

    public FilterRecipeUseCase(
        IMapper mapper,
        ILoggedUser loggedUser,
        IRecipeReadOnlyRepository readOnlyRepository,
        IBlobStorageService blobStorageService)
    {
        _mapper = mapper;
        _loggedUser = loggedUser;
        _readOnlyRepository = readOnlyRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<RecipesResponseJson> Execute(RecipeFilterRequestJson request)
    {
        Validate(request);

        var user = await _loggedUser.User();

        var filters = new RecipeFiltersDto
        {
            RecipeTitleOrIngredient = request.RecipeTitleOrIngredient,
            CookingTimes = request.CookingTimes.Distinct().Select(c => (CookingTime)c).ToList(),
            Difficulties = request.Difficulties.Distinct().Select(d => (Difficulty)d).ToList(),
            DishTypes = request.DishTypes.Distinct().Select(dt => (DishType)dt).ToList()
        };

        var recipes = await _readOnlyRepository.Filter(user, filters);

        return new RecipesResponseJson
        {
            Recipes = await recipes.MapToShortRecipe(user, _mapper, _blobStorageService)
        };
    }

    private static void Validate(RecipeFilterRequestJson request)
    {
        var result = new FilterRecipeValidator().Validate(request);

        if (result.IsValid) return;

        var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();

        throw new ErrorOnValidationException(errorMessages);
    }
}