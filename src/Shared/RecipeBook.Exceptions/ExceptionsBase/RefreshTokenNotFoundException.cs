using System.Net;

namespace RecipeBook.Exceptions.ExceptionsBase;

public class RefreshTokenNotFoundException() : RecipeBookException(ResourceMessageExceptions.REFRESH_TOKEN_NOT_FOUND)
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