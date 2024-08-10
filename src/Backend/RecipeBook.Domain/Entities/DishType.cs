namespace RecipeBook.Domain.Entities;

public class DishType : EntityBase
{
    public Enums.DishType Type { get; set; }
    public long RecipeId { get; set; }
}