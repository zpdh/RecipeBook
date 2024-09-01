using RecipeBook.Communication.Enums;

namespace RecipeBook.Communication.Requests;

public class RecipeFilterRequestJson
{
    public string? RecipeTitleOrIngredient { get; set; }
    public IList<CookingTime> CookingTimes { get; set; } = [];
    public IList<Difficulty> Difficulties { get; set; } = [];
    public IList<DishType> DishTypes { get; set; } = [];
}