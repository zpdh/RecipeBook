using FluentValidation;
using RecipeBook.Application.SharedValidators;
using RecipeBook.Communication.Requests;

namespace RecipeBook.Application.UseCases.User.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestJson>
{
    public ChangePasswordValidator()
    {
        RuleFor(req => req.NewPassword)
            .SetValidator(new PasswordValidator<ChangePasswordRequestJson>());
    }
}