namespace RecipeBook.Communication.Responses;

public class RegisterUserResponseJson
{
    public string Name { get; set; } = string.Empty;
    public TokensResponseJson Tokens { get; set; } = default!;
}