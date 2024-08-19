namespace RecipeBook.Communication.Requests;

public class GenerateRecipeRequestJson
{
    public IList<string> Ingredients { get; set; } = [];
}