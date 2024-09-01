using RecipeBook.Communication.Enums;

namespace RecipeBook.Communication.Requests;

public class RecipeRequestJson
{
    public string Title { get; set; } = string.Empty;
    public CookingTime? CookingTime { get; set; }
    public Difficulty? Difficulty { get; set; }
    public IList<string> Ingredients { get; set; } = [];
    public IList<InstructionRequestJson> Instructions { get; set; } = [];
    public IList<DishType> DishTypes { get; set; } = [];
}