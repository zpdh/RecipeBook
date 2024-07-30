namespace RecipeBook.Domain.Entities;

public abstract class EntityBase
{
    public long Id { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

}