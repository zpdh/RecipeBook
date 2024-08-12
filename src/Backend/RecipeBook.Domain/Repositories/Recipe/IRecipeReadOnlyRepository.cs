using RecipeBook.Domain.DTOs;

namespace RecipeBook.Domain.Repositories.Recipe;

public interface IRecipeReadOnlyRepository
{
    Task<IList<Entities.Recipe>> Filter(Entities.User user, RecipeFiltersDto filters);
}