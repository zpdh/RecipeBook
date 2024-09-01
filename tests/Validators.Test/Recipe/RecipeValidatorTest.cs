using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.Recipe;
using RecipeBook.Communication.Enums;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace Validators.test.Recipe;

public class RecipeValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void NullCookingTimeSuccess()
    {
        var validator = new RecipeValidator();
        var request = RecipeRequestJsonBuilder.Build();
        request.CookingTime = null;

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void NullDifficultySuccess()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Difficulty = null;

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DishTypesNullSuccess()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.DishTypes.Clear();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("         ")]
    public void EmptyTitleError(string title)
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Title = title;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.RECIPE_TITLE_EMPTY));
    }

    [Fact]
    public void InvalidCookingTimeError()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.CookingTime = (CookingTime?)999;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.NOT_ENUM));
    }

    [Fact]
    public void InvalidDifficultyError()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Difficulty = (Difficulty?)999;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.NOT_ENUM));
    }

    [Fact]
    public void InvalidDishTypeError()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.DishTypes.Add((DishType)999);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.NOT_ENUM));
    }

    [Fact]
    public void NoIngredientsError()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Ingredients.Clear();

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.INGREDIENT_LIST_EMPTY));
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    public void EmptyIngredientError(string ingredient)
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Ingredients.Add(ingredient);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.INGREDIENT_EMPTY));
    }

    [Fact]
    public void NoInstructionsError()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Instructions.Clear();

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.INSTRUCTION_LIST_EMPTY));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void EmptyInstructionError(string text)
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Instructions.Add(new InstructionRequestJson
        {
            Step = 4,
            Text = text
        });

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.INSTRUCTION_EMPTY));
    }

    [Fact]
    public void InvalidInstructionStepError()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Instructions.Add(new InstructionRequestJson
        {
            Step = -213,
            Text = "lorem"
        });

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.INVALID_INSTRUCTION_INT));
    }

    [Fact]
    public void DuplicatedInstructionError()
    {
        var validator = new RecipeValidator();

        var request = RecipeRequestJsonBuilder.Build();
        request.Instructions.First().Step = request.Instructions.Last().Step;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.INSTRUCTION_DUPLICATED));
    }

    [Fact]
    public void InstructionTooLongError()
    {
        var request = RecipeRequestJsonBuilder.Build();
        request.Instructions.First().Text = StringGeneratorRequest.LongString(minCharacters: 2001);

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.INSTRUCTION_TOO_LONG));
    }
}