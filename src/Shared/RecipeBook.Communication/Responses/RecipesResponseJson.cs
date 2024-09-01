namespace RecipeBook.Communication.Responses;

public class RecipesResponseJson
{
    public IList<ShortRecipeResponseJson> Recipes { get; set; } = [];
}