using System.Net;

namespace RecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException(IList<string> errorMessages) : RecipeBookException(string.Empty)
{
    public override IList<string> GetErrorMessages()
    {
        return errorMessages;
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.BadRequest;
    }
}