using System.Net;

namespace RecipeBook.Exceptions.ExceptionsBase;

public class NotFoundException(string message) : RecipeBookException(message)
{
    public override IList<string> GetErrorMessages()
    {
        return [Message];
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.NotFound;
    }
}