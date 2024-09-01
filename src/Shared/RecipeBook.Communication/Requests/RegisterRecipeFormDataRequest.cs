using Microsoft.AspNetCore.Http;

namespace RecipeBook.Communication.Requests;

public class RegisterRecipeFormDataRequest : RecipeRequestJson
{
    public IFormFile? Image { get; set; }
}