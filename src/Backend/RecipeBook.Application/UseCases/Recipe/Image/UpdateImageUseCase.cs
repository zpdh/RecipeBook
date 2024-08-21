using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using Microsoft.AspNetCore.Http;
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
            throw new NotFoundException("Recipe not found");
        }

        var fileStream = file.OpenReadStream();

        if (IsNotImage(fileStream))
        {
            throw new ErrorOnValidationException([ResourceMessageExceptions.ONLY_IMAGES_ACCEPTED]);
        }

        if (string.IsNullOrEmpty(recipe.ImageIdentifier))
        {
            recipe.ImageIdentifier = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            _repository.Update(recipe);

            await _unitOfWork.Commit();
        }

        fileStream.Position = 0;

        await _blobStorageService.Upload(user, fileStream, recipe.ImageIdentifier);
    }

    private static bool IsNotImage(Stream fileStream)
    {
        return !fileStream.Is<PortableNetworkGraphic>()
               && !fileStream.Is<JointPhotographicExpertsGroup>();
    }
}