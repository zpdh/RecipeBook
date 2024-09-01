namespace RecipeBook.Communication.Requests;

public class InstructionRequestJson
{
    public int Step { get; set; }
    public string Text { get; set; } = string.Empty;
}