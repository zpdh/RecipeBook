using System.Net;

namespace RecipeBook.Exceptions.ExceptionsBase;

public abstract class RecipeBookException(string message) : SystemException(message)
{
    public abstract IList<string> GetErrorMessages();
    public abstract HttpStatusCode GetStatusCode();
}