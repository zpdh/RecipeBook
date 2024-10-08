using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using Microsoft.AspNetCore.Http;
using RecipeBook.Application.Extensions;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Domain.Services.Storage;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.Image;

public class UpdateImageUseCase : IUpdateImageUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public UpdateImageUseCase(
        ILoggedUser loggedUser,
        IRecipeUpdateOnlyRepository repository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task Execute(long id, IFormFile file)
    {
        var user = await _loggedUser.User();
        var recipe = await _repository.GetById(user, id);

        if (recipe is null)
        {
            throw new NotFoundException(ResourceMessageExceptions.RECIPE_NOT_FOUND);
        }

        var fileStream = file.OpenReadStream();
        var (isStreamValid, extension) = fileStream.ValidateAndGetImageExtension();

        if (isStreamValid.IsFalse())
        {
            throw new ErrorOnValidationException([ResourceMessageExceptions.ONLY_IMAGES_ACCEPTED]);
        }

        if (string.IsNullOrEmpty(recipe.ImageIdentifier))
        {
            recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

            _repository.Update(recipe);

            await _unitOfWork.Commit();
        }

        await _blobStorageService.Upload(user, fileStream, recipe.ImageIdentifier);
    }
}