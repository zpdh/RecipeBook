using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repositories;
using FluentAssertions;
using RecipeBook.Application.UseCases.Dashboard.Get;

namespace UseCases.Tests.Dashboard.GetDashboard;

public class GetDashboardUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.Collection(user, 4);

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Execute();

        result.Should().NotBeNull();
        result.Recipes
            .Should().HaveCountGreaterThan(0)
            .And.OnlyHaveUniqueItems(recipe => recipe.Id)
            .And.AllSatisfy(recipe =>
            {
                recipe.Id.Should().NotBeNullOrWhiteSpace();
                recipe.Title.Should().NotBeNullOrWhiteSpace();
                recipe.IngredientAmount.Should().BeGreaterThan(0);
            });
    }

    private static GetDashboardUseCase CreateUseCase(
        RecipeBook.Domain.Entities.User user,
        IList<RecipeBook.Domain.Entities.Recipe> recipes)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var readRepo = new RecipeReadOnlyRepositoryBuilder().GetDashboardRecipes(user, recipes).Build();

        return new GetDashboardUseCase(loggedUser, mapper, readRepo);
    }
}