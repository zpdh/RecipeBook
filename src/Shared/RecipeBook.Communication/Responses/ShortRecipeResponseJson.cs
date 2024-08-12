namespace RecipeBook.Communication.Responses;

public class ShortRecipeResponseJson
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int IngredientAmount { get; set; }
}