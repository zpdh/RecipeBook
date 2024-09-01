using FluentValidation;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace RecipeBook.Application.UseCases.Recipe;

public class RecipeValidator : AbstractValidator<RecipeRequestJson>
{
    public RecipeValidator()
    {
        RuleFor(recipe => recipe.Title)
            .NotEmpty().WithMessage(ResourceMessageExceptions.RECIPE_TITLE_EMPTY);

        RuleFor(recipe => recipe.CookingTime)
            .IsInEnum().WithMessage(ResourceMessageExceptions.NOT_ENUM);

        RuleFor(recipe => recipe.Difficulty)
            .IsInEnum().WithMessage(ResourceMessageExceptions.NOT_ENUM);

        RuleFor(recipe => recipe.Ingredients.Count)
            .GreaterThan(0).WithMessage(ResourceMessageExceptions.INGREDIENT_LIST_EMPTY);

        RuleForEach(recipe => recipe.Ingredients)
            .NotEmpty().WithMessage(ResourceMessageExceptions.INGREDIENT_EMPTY);

        RuleFor(recipe => recipe.Instructions.Count)
            .GreaterThan(0).WithMessage(ResourceMessageExceptions.INSTRUCTION_LIST_EMPTY);

        RuleForEach(recipe => recipe.Instructions).ChildRules(instructionRule =>
        {
            instructionRule.RuleFor(instruction => instruction.Step)
                .GreaterThan(0).WithMessage(ResourceMessageExceptions.INVALID_INSTRUCTION_INT);

            instructionRule.RuleFor(instruction => instruction.Text)
                .NotEmpty().WithMessage(ResourceMessageExceptions.INSTRUCTION_EMPTY)
                .MaximumLength(2000).WithMessage(ResourceMessageExceptions.INSTRUCTION_TOO_LONG);
        });
        
        /*
         * Kind of confusing. Takes the list of instructions, gets a
         * list of distinct instructions value-wise, then checks
         * if the distinct lists size is equal to the instruction
         * list size. Made to prevent cases where the user inputs
         * 2 instructions with the same integer value.
         */
        RuleFor(recipe => recipe.Instructions)
            .Must(instructions =>
                instructions.Select(instruction => instruction.Step).Distinct().Count() == instructions.Count)
            .WithMessage(ResourceMessageExceptions.INSTRUCTION_DUPLICATED);

        RuleForEach(recipe => recipe.DishTypes)
            .IsInEnum().WithMessage(ResourceMessageExceptions.NOT_ENUM);
    }
}