using Moq;
using RecipeBook.Domain.DTOs;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtils.Repositories;

public class RecipeReadOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeReadOnlyRepository> _repository;

    public RecipeReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IRecipeReadOnlyRepository>();
    }

    public RecipeReadOnlyRepositoryBuilder Filter(User user, IList<Recipe> recipes)
    {
        _repository.Setup(repository =>
            repository.Filter(user, It.IsAny<RecipeFiltersDto>())).ReturnsAsync(recipes);

        return this;
    }

    public RecipeReadOnlyRepositoryBuilder GetById(User user, Recipe? recipe)
    {
        if (recipe is not null)
        {
            _repository.Setup(repository => repository.GetById(user, recipe.Id)).ReturnsAsync(recipe);
        }

        return this;
    }

    public RecipeReadOnlyRepositoryBuilder GetDashboardRecipes(User user, IList<Recipe> recipes)
    {
        _repository.Setup(repository => repository.GetDashboardRecipes(user)).ReturnsAsync(recipes);

        return this;
    }

    public IRecipeReadOnlyRepository Build()
    {
        return _repository.Object;
    }
}