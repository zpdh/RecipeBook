using System.Net;

namespace RecipeBook.Exceptions.ExceptionsBase;

public class InvalidLoginException() : RecipeBookException(ResourceMessageExceptions.EMAIL_OR_PASSWORD_INVALID)
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