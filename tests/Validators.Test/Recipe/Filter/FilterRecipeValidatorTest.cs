using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.Recipe.Filter;
using RecipeBook.Communication.Enums;
using RecipeBook.Exceptions;

namespace Validators.test.Recipe.Filter;

public class FilterRecipeValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new FilterRecipeValidator();

        var request = RecipeFilterRequestJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void InvalidCookingTimeError()
    {
        var validator = new FilterRecipeValidator();

        var request = RecipeFilterRequestJsonBuilder.Build();
        request.CookingTimes.Add((CookingTime) 999);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.NOT_ENUM));
    }

    [Fact]
    public void InvalidDifficultyError()
    {
        var validator = new FilterRecipeValidator();

        var request = RecipeFilterRequestJsonBuilder.Build();
        request.Difficulties.Add((Difficulty) 999);

        var result = validator.Validate(request);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.NOT_ENUM));
    }

    [Fact]
    public void InvalidDishTypeError()
    {
        var validator = new FilterRecipeValidator();

        var request = RecipeFilterRequestJsonBuilder.Build();
        request.DishTypes.Add((DishType) 999);

        var result = validator.Validate(request);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.NOT_ENUM));
    }
}