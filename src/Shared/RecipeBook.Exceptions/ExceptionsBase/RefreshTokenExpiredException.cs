using System.Net;

namespace RecipeBook.Exceptions.ExceptionsBase;

public class RefreshTokenExpiredException() : RecipeBookException(ResourceMessageExceptions.REFRESH_TOKEN_EXPIRED)
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