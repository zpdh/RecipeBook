using RecipeBook.Domain.Enums;

namespace RecipeBook.Domain.DTOs;

public record RecipeFiltersDto
{
    public string? RecipeTitleOrIngredient { get; init; }
    public IList<CookingTime> CookingTimes { get; init; } = [];
    public IList<Difficulty> Difficulties { get; init; } = [];
    public IList<DishType> DishTypes { get; init; } = [];
}