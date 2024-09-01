using CommonTestUtils.Blob;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Repositories;
using FluentAssertions;
using RecipeBook.Application.UseCases.Recipe.Delete;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.Recipe.Delete;

public class DeleteRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipe.Id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RecipeNotFoundError()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(1);

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        && e.GetErrorMessages().Contains(ResourceMessageExceptions.RECIPE_NOT_FOUND));
    }

    private static DeleteRecipeUseCase CreateUseCase(
        RecipeBook.Domain.Entities.User user,
        RecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var readRepo = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var writeRepo = RecipeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();

        return new DeleteRecipeUseCase(loggedUser, unitOfWork, readRepo, writeRepo, blobStorage);
    }
}