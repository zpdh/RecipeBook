using System.Net;

namespace RecipeBook.Exceptions.ExceptionsBase;

public class RecipeBookAuthenticationException(string message) : RecipeBookException(message)
{
    public override IList<string> GetErrorMessages()
    {
        return [Message];
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.Unauthorized;
    }
}