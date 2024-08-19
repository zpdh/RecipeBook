using FluentValidation;
using RecipeBook.Communication.Requests;
using RecipeBook.Domain.ValueObjects;
using RecipeBook.Exceptions;

namespace RecipeBook.Application.UseCases.Recipe.Generate;

public class GenerateRecipeValidator : AbstractValidator<GenerateRecipeRequestJson>
{
    public GenerateRecipeValidator()
    {
        var maximumIngredients = RuleConstants.MaximumIngredientsRecipeGeneration;

        RuleFor(recipe => recipe.Ingredients.Count)
            .InclusiveBetween(1, maximumIngredients)
            .WithMessage(ResourceMessageExceptions.INVALID_NUMBER_OF_INGREDIENTS);

        RuleFor(recipe => recipe.Ingredients)
            .Must(ingredients => ingredients.Count == ingredients.Select(ing => ing).Distinct().Count())
            .WithMessage(ResourceMessageExceptions.DUPLICATE_INGREDIENTS);

        RuleFor(request => request.Ingredients)
            .ForEach(rule =>
            {
                rule.Custom((ingredient, context) =>
                {
                    if (string.IsNullOrWhiteSpace(ingredient))
                    {
                        context.AddFailure("Ingredient", ResourceMessageExceptions.INGREDIENT_EMPTY);
                    }

                    else if (ingredient.Count(chr => chr == ' ') > 3 || ingredient.Count(chr => chr == '/') > 1)
                    {
                        context.AddFailure("Ingredient", ResourceMessageExceptions.INGREDIENT_NOT_FOLLOWING_PATTERN);
                    }
                });
            });
    }
}