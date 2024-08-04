namespace RecipeBook.Exceptions.ExceptionsBase;

public class RecipeBookException : SystemException
{
    protected RecipeBookException(string message) : base(message)
    {
    }
}