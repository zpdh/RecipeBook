using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using Microsoft.AspNetCore.Http;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.Image;

public class UpdateImageUseCase : IUpdateImageUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateImageUseCase(
        ILoggedUser loggedUser,
        IRecipeUpdateOnlyRepository repository,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _unitOfWork = unitOfWork;
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

        if (IsPngOrJpeg(fileStream).IsFalse())
        {
            throw new ErrorOnValidationException(
                [ResourceMessageExceptions.ONLY_IMAGES_ACCEPTED]
            );
        }
    }

    private static bool IsPngOrJpeg(Stream fileStream)
    {
        return fileStream.Is<PortableNetworkGraphic>()
               || fileStream.Is<JointPhotographicExpertsGroup>();
    }
}