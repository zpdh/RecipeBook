using RecipeBook.Domain.Enums;

namespace RecipeBook.Domain.DTOs;

public record GeneratedRecipeDto
{
    public string Title { get; init; } = string.Empty;
    public CookingTime CookingTime { get; init; }
    public IList<string> Ingredients { get; init; } = [];
    public IList<GeneratedInstructionDto> Instructions { get; init; } = [];
}