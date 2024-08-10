namespace RecipeBook.Domain.Entities;

public class Ingredient : EntityBase
{
    public string Item { get; set; } = string.Empty;
    public long RecipeId { get; set; }
}