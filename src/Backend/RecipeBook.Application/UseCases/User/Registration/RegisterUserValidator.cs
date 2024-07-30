using FluentValidation;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace RecipeBook.Application.UseCases.User.Registration;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequestJson>
{
    public RegisterUserValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessageExceptions.NAME_EMPTY);

        RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessageExceptions.EMAIL_EMPTY);
        RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessageExceptions.EMAIL_INVALID);

        RuleFor(user => user.Password.Length).GreaterThanOrEqualTo(6).WithMessage(ResourceMessageExceptions.PASSWORD_LENGTH);
    }
}