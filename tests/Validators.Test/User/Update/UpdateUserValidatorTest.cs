using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.User.Update;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace Validators.test.User.Update;

public class UpdateUserValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateUserValidator();

        var request = UpdateUserRequestJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void NameEmptyError()
    {
        var validator = new UpdateUserValidator();

        var request = UpdateUserRequestJsonBuilder.Build();
        request.Name = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.NAME_EMPTY));
    }

    [Fact]
    public void EmailEmptyError()
    {
        var validator = new UpdateUserValidator();

        var request = UpdateUserRequestJsonBuilder.Build();
        request.Email = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.EMAIL_EMPTY));
    }

    [Fact]
    public void EmailInvalidError()
    {
        var validator = new UpdateUserValidator();

        var request = UpdateUserRequestJsonBuilder.Build();
        var split = request.Email.Split('@');
        request.Email = string.Join(string.Empty, split);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();

        result.Errors
            .Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessageExceptions.EMAIL_INVALID));
    }
}