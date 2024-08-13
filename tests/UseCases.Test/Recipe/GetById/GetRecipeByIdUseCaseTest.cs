using AutoMapper;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repositories;
using FluentAssertions;
using RecipeBook.Application.UseCases.Recipe.GetById;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.Recipe.GetById;

public class GetRecipeByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        var useCase = CreateUseCase(user, recipe);

        var result = await useCase.Execute(recipe.Id);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(recipe.Title);
    }

    [Fact]
    public async Task RecipeNotFoundError()
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(user, null);

        var act = async () => await useCase.Execute(21314134);

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        && e.Message.Equals(ResourceMessageExceptions.RECIPE_NOT_FOUND));
    }

    private static GetRecipeByIdUseCase CreateUseCase(
        RecipeBook.Domain.Entities.User user,
        RecipeBook.Domain.Entities.Recipe? recipe)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var readRepo = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();

        return new GetRecipeByIdUseCase(readRepo, loggedUser, mapper);
    }
}