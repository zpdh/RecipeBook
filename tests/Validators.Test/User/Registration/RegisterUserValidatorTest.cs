using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.User.Registration;
using RecipeBook.Exceptions;

namespace Validators.test.User.Registration;

public class RegisterUserValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RegisterUserValidator();

        var request = RegisterUserRequestJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void NameEmptyError()
    {
        var validator = new RegisterUserValidator();

        var request = RegisterUserRequestJsonBuilder.Build();
        request.Name = "";

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.NAME_EMPTY));
    }

    [Fact]
    public void EmptyEmailError()
    {
        var validator = new RegisterUserValidator();

        var request = RegisterUserRequestJsonBuilder.Build();
        request.Email = "";

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.EMAIL_EMPTY));
    }

    [Fact]
    public void InvalidEmailError()
    {
        var validator = new RegisterUserValidator();

        var request = RegisterUserRequestJsonBuilder.Build();
        request.Email = "invalid_email.com";

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.EMAIL_INVALID));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void InvalidPasswordError(int passwordLength)
    {
        var validator = new RegisterUserValidator();

        var request = RegisterUserRequestJsonBuilder.Build(passwordLength);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And
            .Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.PASSWORD_LENGTH));
    }
}