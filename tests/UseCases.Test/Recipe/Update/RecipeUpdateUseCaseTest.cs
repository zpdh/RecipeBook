using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.Recipe.Update;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.Recipe.Update;

public class RecipeUpdateUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        var newRecipe = RecipeRequestJsonBuilder.Build();

        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipe.Id, newRecipe);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RecipeNotFoundError()
    {
        var (user, _) = UserBuilder.Build();
        var newRecipe = RecipeRequestJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(1, newRecipe);

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        && e.GetErrorMessages().Contains(ResourceMessageExceptions.RECIPE_NOT_FOUND));
    }

    [Fact]
    public async Task EmptyTitleError()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        var newRecipe = RecipeRequestJsonBuilder.Build();

        newRecipe.Title = string.Empty;

        var useCase = CreateUseCase(user, recipe);

        var act = async () => await useCase.Execute(recipe.Id, newRecipe);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        && e.GetErrorMessages().Contains(ResourceMessageExceptions.RECIPE_TITLE_EMPTY));
    }

    private static UpdateRecipeUseCase CreateUseCase(
        RecipeBook.Domain.Entities.User user,
        RecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var updateRepo = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();

        return new UpdateRecipeUseCase(loggedUser, updateRepo, unitOfWork, mapper);
    }
}