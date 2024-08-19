using RecipeBook.Communication.Enums;

namespace RecipeBook.Communication.Responses;

public class GenerateRecipeResponseJson
{
    public string Title { get; set; } = string.Empty;
    public IList<string> Ingredients { get; set; } = [];
    public IList<GeneratedInstructionResponseJson> Instructions { get; set; } = [];
    public CookingTime CookingTime { get; set; }
    public Difficulty Difficulty { get; set; }
}