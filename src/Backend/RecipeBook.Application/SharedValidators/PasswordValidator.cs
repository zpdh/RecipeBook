using FluentValidation;
using FluentValidation.Validators;
using RecipeBook.Domain.Extensions;
using RecipeBook.Exceptions;

namespace RecipeBook.Application.SharedValidators;

public class PasswordValidator<TRequest> : PropertyValidator<TRequest, string>
{
    public override string Name => "PasswordValidator";

    public override bool IsValid(ValidationContext<TRequest> context, string password)
    {
        return ValidatePasswordNull(context, password)
               && ValidatePasswordLength(context, password);
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "{ErrorMessage}";
    }

    private static bool ValidatePasswordLength(ValidationContext<TRequest> context, string password)
    {
        if (password.Length >= 6) return true;

        context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessageExceptions.PASSWORD_LENGTH);

        return false;
    }

    private static bool ValidatePasswordNull(ValidationContext<TRequest> context, string password)
    {
        if (password.IsNotEmpty()) return true;

        context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessageExceptions.PASSWORD_INVALID);

        return false;
    }
}