using FluentValidation;
using RecipeBook.Communication.Requests;
using RecipeBook.Domain.Extensions;
using RecipeBook.Exceptions;

namespace RecipeBook.Application.UseCases.User.Update;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequestJson>
{
    public UpdateUserValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(ResourceMessageExceptions.NAME_EMPTY);

        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage(ResourceMessageExceptions.EMAIL_EMPTY);

        When(request => request.Email.IsNotEmpty(), () =>
        {
            RuleFor(request => request.Email)
                .EmailAddress()
                .WithMessage(ResourceMessageExceptions.EMAIL_INVALID);
        });
    }
}