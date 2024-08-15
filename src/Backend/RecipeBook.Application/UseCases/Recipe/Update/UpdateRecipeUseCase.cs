using AutoMapper;
using RecipeBook.Communication.Requests;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.Update;

public class UpdateRecipeUseCase : IUpdateRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeUpdateOnlyRepository _updateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateRecipeUseCase(
        ILoggedUser loggedUser,
        IRecipeUpdateOnlyRepository updateOnlyRepository,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _loggedUser = loggedUser;
        _updateOnlyRepository = updateOnlyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Execute(long recipeId, RecipeRequestJson request)
    {
        Validate(request);

        var user = await _loggedUser.User();
        var recipe = await _updateOnlyRepository.GetById(user, recipeId);

        if (recipe is null)
        {
            throw new NotFoundException(ResourceMessageExceptions.RECIPE_NOT_FOUND);
        }

        recipe.Ingredients.Clear();
        recipe.DishTypes.Clear();
        recipe.Instructions.Clear();

        _mapper.Map(request, recipe);

        var instructions = request.Instructions.OrderBy(instruction => instruction.Step).ToList();
        for (var i = 0; i < instructions.Count; i++)
        {
            instructions[i].Step = i + 1;
        }

        recipe.Instructions = _mapper.Map<IList<Instruction>>(instructions);

        _updateOnlyRepository.Update(recipe);
        await _unitOfWork.Commit();
    }

    private static void Validate(RecipeRequestJson request)
    {
        var result = new RecipeValidator().Validate(request);

        if (result.IsValid.IsFalse())
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
        }
    }
}