using AutoMapper;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Domain.Services.Storage;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.GetById;

public class GetRecipeByIdUseCase : IGetRecipeByIdUseCase
{
    private readonly IRecipeReadOnlyRepository _readOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;


    public GetRecipeByIdUseCase(
        IRecipeReadOnlyRepository readOnlyRepository,
        ILoggedUser loggedUser,
        IMapper mapper,
        IBlobStorageService blobStorageService)
    {
        _readOnlyRepository = readOnlyRepository;
        _loggedUser = loggedUser;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }

    public async Task<RecipeResponseJson> Execute(long recipeId)
    {
        var user = await _loggedUser.User();
        var recipe = await _readOnlyRepository.GetById(user, recipeId);

        if (recipe is null)
        {
            throw new NotFoundException(ResourceMessageExceptions.RECIPE_NOT_FOUND);
        }

        var response = _mapper.Map<RecipeResponseJson>(recipe);

        if (recipe.ImageIdentifier.IsNotEmpty())
        {
            var url = await _blobStorageService.GetFileUrl(user, recipe.ImageIdentifier);

            response.ImageUrl = url;
        }

        return response;
    }
}