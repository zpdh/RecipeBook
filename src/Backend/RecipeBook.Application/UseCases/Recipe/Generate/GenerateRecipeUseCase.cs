using RecipeBook.Communication.Enums;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Services.OpenAI;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.Generate;

public class GenerateRecipeUseCase : IGenerateRecipeUseCase
{
    private readonly IGenerateRecipeAI _generator;

    public GenerateRecipeUseCase(IGenerateRecipeAI generator)
    {
        _generator = generator;
    }


    public async Task<GenerateRecipeResponseJson> Execute(GenerateRecipeRequestJson request)
    {
        Validate(request);

        var response = await _generator.Generate(request.Ingredients);

        return new GenerateRecipeResponseJson
        {
            Title = response.Title,
            Ingredients = response.Ingredients,
            CookingTime = (CookingTime)response.CookingTime,
            Instructions = response.Instructions.Select(instruction => new GeneratedInstructionResponseJson
            {
                Step = instruction.Step,
                Text = instruction.Text
            }).ToList(),
            Difficulty = Difficulty.Low
        };
    }

    private static void Validate(GenerateRecipeRequestJson request)
    {
        var result = new GenerateRecipeValidator().Validate(request);

        if (result.IsValid) return;

        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();

        throw new ErrorOnValidationException(errors);
    }
}