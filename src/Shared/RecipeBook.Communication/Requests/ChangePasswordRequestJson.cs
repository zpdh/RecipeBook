namespace RecipeBook.Communication.Requests;

public class ChangePasswordRequestJson
{
    public string Password { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}