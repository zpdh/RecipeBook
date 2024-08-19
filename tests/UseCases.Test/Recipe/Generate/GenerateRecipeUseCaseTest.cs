using CommonTestUtils.DTOs;
using CommonTestUtils.OpenAI;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.Recipe.Generate;
using RecipeBook.Communication.Enums;
using RecipeBook.Domain.DTOs;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.Recipe.Generate;

public class GenerateRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var dto = GeneratedRecipeDtoBuilder.Build();
        var request = GenerateRecipeRequestJsonBuilder.Build();
        var useCase = CreateUseCase(dto);

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.CookingTime.Should().Be((CookingTime)dto.CookingTime);
        result.Difficulty.Should().Be(Difficulty.Low);
    }

    [Fact]
    public async Task DuplicatedIngredientsError()
    {
        var dto = GeneratedRecipeDtoBuilder.Build();
        var request = GenerateRecipeRequestJsonBuilder.Build();
        request.Ingredients[0] = request.Ingredients.Last();
        var useCase = CreateUseCase(dto);

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        && e.GetErrorMessages().Contains(ResourceMessageExceptions.DUPLICATE_INGREDIENTS));
    }

    private static GenerateRecipeUseCase CreateUseCase(GeneratedRecipeDto dto)
    {
        var generator = GenerateRecipeAIBuilder.Build(dto);

        return new GenerateRecipeUseCase(generator);
    }
}