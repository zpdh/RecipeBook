namespace RecipeBook.Communication.Requests;

public class LoginRequestJson
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}