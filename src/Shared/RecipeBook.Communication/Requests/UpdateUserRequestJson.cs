namespace RecipeBook.Communication.Requests;

public class UpdateUserRequestJson
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}