using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.Recipe.Filter;
using RecipeBook.Communication.Enums;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.Recipe.Filter;

public class FilterRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var request = RecipeFilterRequestJsonBuilder.Build();

        var recipes = RecipeBuilder.Collection(user);

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Recipes.Should().NotBeNullOrEmpty();
        result.Recipes.Should().HaveCount(recipes.Count);
    }

    [Fact]
    public async Task InvalidCookingTimeError()
    {
        var (user, _) = UserBuilder.Build();

        var request = RecipeFilterRequestJsonBuilder.Build();
        request.CookingTimes.Add((CookingTime)999);

        var recipes = RecipeBuilder.Collection(user);

        var useCase = CreateUseCase(user, recipes);

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>()).Where(e =>
            e.GetErrorMessages().Count == 1
            && e.GetErrorMessages().Contains(ResourceMessageExceptions.NOT_ENUM));
    }

    private static FilterRecipeUseCase CreateUseCase(
        RecipeBook.Domain.Entities.User user,
        IList<RecipeBook.Domain.Entities.Recipe> recipes
    )
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var readRepo = new IRecipeReadOnlyRepositoryBuilder().Filter(user, recipes).Build();

        return new FilterRecipeUseCase(mapper, loggedUser, readRepo);
    }
}