using RecipeBook.Communication.Enums;

namespace RecipeBook.Communication.Responses;

public class RecipeResponseJson
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public IList<IngredientResponseJson> Ingredients { get; set; } = [];
    public IList<InstructionResponseJson> Instructions { get; set; } = [];
    public IList<DishType> DishTypes { get; set; } = [];
    public CookingTime? CookingTime { get; set; }
    public Difficulty? Difficulty { get; set; }
}