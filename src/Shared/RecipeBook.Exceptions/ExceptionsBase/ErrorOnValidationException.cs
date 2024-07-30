namespace RecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException : RecipeBookException
{
    public IList<string> ErrorMessages { get; set; }

    public ErrorOnValidationException(IList<string> errorMessages)
    {
        ErrorMessages = errorMessages;
    }
}