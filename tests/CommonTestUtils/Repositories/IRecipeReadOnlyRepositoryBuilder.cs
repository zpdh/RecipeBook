using Moq;
using RecipeBook.Domain.DTOs;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtils.Repositories;

public class IRecipeReadOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeReadOnlyRepository> _repository;

    public IRecipeReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IRecipeReadOnlyRepository>();
    }

    public IRecipeReadOnlyRepositoryBuilder Filter(User user, IList<Recipe> recipes)
    {
        _repository.Setup(repository =>
            repository.Filter(user, It.IsAny<RecipeFiltersDto>())).ReturnsAsync(recipes);

        return this;
    }

    public IRecipeReadOnlyRepository Build()
    {
        return _repository.Object;
    }
}