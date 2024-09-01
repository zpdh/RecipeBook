using Moq;
using RecipeBook.Domain.DTOs;
using RecipeBook.Domain.Services.OpenAI;

namespace CommonTestUtils.OpenAI;

public class GenerateRecipeAIBuilder
{
    public static IGenerateRecipeAI Build(GeneratedRecipeDto dto)
    {
        var mock = new Mock<IGenerateRecipeAI>();

        mock.Setup(service => service.Generate(It.IsAny<List<string>>())).ReturnsAsync(dto);

        return mock.Object;
    }
}