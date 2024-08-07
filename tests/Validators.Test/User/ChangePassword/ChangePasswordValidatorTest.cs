using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.SharedValidators;
using RecipeBook.Application.UseCases.User.ChangePassword;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace Validators.test.User.ChangePassword;

public class ChangePasswordValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new ChangePasswordValidator();

        var request = ChangePasswordRequestJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void PasswordLengthError(int passwordLength)
    {
        var validator = new ChangePasswordValidator();

        var request = ChangePasswordRequestJsonBuilder.Build(passwordLength);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.PASSWORD_LENGTH));
    }

    [Fact]
    public void InvalidPasswordError()
    {
        var validator = new ChangePasswordValidator();

        var request = ChangePasswordRequestJsonBuilder.Build(0);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.PASSWORD_INVALID));
    }
}