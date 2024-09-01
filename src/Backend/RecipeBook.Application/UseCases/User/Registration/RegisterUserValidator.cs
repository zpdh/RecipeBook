using FluentValidation;
using RecipeBook.Application.SharedValidators;
using RecipeBook.Communication.Requests;
using RecipeBook.Domain.Extensions;
using RecipeBook.Exceptions;

namespace RecipeBook.Application.UseCases.User.Registration;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequestJson>
{
    public RegisterUserValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessageExceptions.NAME_EMPTY);

        RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessageExceptions.EMAIL_EMPTY);
        When(user => user.Email.IsNotEmpty(),
            () => RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessageExceptions.EMAIL_INVALID));

        RuleFor(user => user.Password).SetValidator(new PasswordValidator<RegisterUserRequestJson>());
    }
}