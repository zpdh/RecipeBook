using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeBook.Domain.Entities;

[Table("DishTypes")]
public class DishType : EntityBase
{
    public Enums.DishType Type { get; set; }
    public long RecipeId { get; set; }
}