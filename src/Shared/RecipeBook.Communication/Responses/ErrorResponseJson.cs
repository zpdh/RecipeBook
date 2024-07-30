namespace RecipeBook.Communication.Responses;

public class ErrorResponseJson
{
    public IList<string> ErrorMessages { get; set; }

    public ErrorResponseJson(IList<string> errorMessages) => ErrorMessages = errorMessages;

    public ErrorResponseJson(string errorMessage)
    {
        ErrorMessages = new List<string>
        {
            errorMessage
        };
    }
}