using FluentValidation;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace RecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipeValidator : AbstractValidator<RecipeFilterRequestJson>
{
    public FilterRecipeValidator()
    {
        RuleForEach(r => r.CookingTimes).IsInEnum().WithMessage(ResourceMessageExceptions.NOT_ENUM);
        RuleForEach(r => r.Difficulties).IsInEnum().WithMessage(ResourceMessageExceptions.NOT_ENUM);
        RuleForEach(r => r.DishTypes).IsInEnum().WithMessage(ResourceMessageExceptions.NOT_ENUM);
    }
}