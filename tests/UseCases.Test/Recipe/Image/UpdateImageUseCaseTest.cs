using CommonTestUtils.Blob;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using RecipeBook.Application.UseCases.Recipe.Image;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;
using UseCases.Tests.Recipe.InlineData;

namespace UseCases.Tests.Recipe.Image;

public class UpdateImageUseCaseTest
{
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success(IFormFile file)
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipe.Id, file);

        await act.Should().NotThrowAsync();
    }

    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task RecipeNotFoundError(IFormFile file)
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(123, file);

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessageExceptions.RECIPE_NOT_FOUND));
    }

    [Fact]
    public async Task InvalidFileError()
    {
        var (user, _) = UserBuilder.Build();
        var file = FormFileBuilder.Txt();
        var recipe = RecipeBuilder.Build(user);
        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipe.Id, file);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessageExceptions.ONLY_IMAGES_ACCEPTED));
    }

    private static UpdateImageUseCase CreateUseCase(RecipeBook.Domain.Entities.User user,
        RecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var updateRepo = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();

        return new UpdateImageUseCase(loggedUser, updateRepo, unitOfWork, blobStorage);
    }
}