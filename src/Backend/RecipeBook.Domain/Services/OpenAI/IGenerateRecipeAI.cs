using RecipeBook.Domain.DTOs;

namespace RecipeBook.Domain.Services.OpenAI;

public interface IGenerateRecipeAI
{
    Task<GeneratedRecipeDto> Generate(IList<string> ingredients);
}