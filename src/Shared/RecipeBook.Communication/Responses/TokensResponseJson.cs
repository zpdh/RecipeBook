namespace RecipeBook.Communication.Responses;

public class TokensResponseJson
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}