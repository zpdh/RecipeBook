using AutoMapper;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.GetById;

public class GetRecipeByIdUseCase : IGetRecipeByIdUseCase
{
    private readonly IRecipeReadOnlyRepository _readOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;


    public GetRecipeByIdUseCase(IRecipeReadOnlyRepository readOnlyRepository, ILoggedUser loggedUser, IMapper mapper)
    {
        _readOnlyRepository = readOnlyRepository;
        _loggedUser = loggedUser;
        _mapper = mapper;
    }

    public async Task<RecipeResponseJson> Execute(long recipeId)
    {
        var user = await _loggedUser.User();
        var recipe = await _readOnlyRepository.GetById(user, recipeId);

        if (recipe is null)
        {
            throw new NotFoundException(ResourceMessageExceptions.RECIPE_NOT_FOUND);
        }

        return _mapper.Map<RecipeResponseJson>(recipe);
    }
}