using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Domain.Services.Storage;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.Delete;

public class DeleteRecipeUseCase : IDeleteRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRecipeReadOnlyRepository _readOnlyRepository;
    private readonly IRecipeWriteOnlyRepository _writeOnlyRepository;
    private readonly IBlobStorageService _blobStorageService;

    public DeleteRecipeUseCase(
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork,
        IRecipeReadOnlyRepository readOnlyRepository,
        IRecipeWriteOnlyRepository writeOnlyRepository,
        IBlobStorageService blobStorageService)
    {
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task Execute(long id)
    {
        var user = await _loggedUser.User();
        var recipe = await _readOnlyRepository.GetById(user, id);

        if (recipe is null)
        {
            throw new NotFoundException(ResourceMessageExceptions.RECIPE_NOT_FOUND);
        }

        if (recipe.ImageIdentifier.IsNotEmpty())
        {
            await _blobStorageService.Delete(user, recipe.ImageIdentifier);
        }

        await _writeOnlyRepository.Delete(id);
        await _unitOfWork.Commit();
    }
}