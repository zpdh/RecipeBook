namespace RecipeBook.Domain.Extensions;

public static class BooleanExtension
{
    public static bool IsFalse(this bool value)
    {
        // Pretty much a guard clause; Makes code reading easier
        return !value;
    }
}