using CommonTestUtils.Cryptography;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.User.ChangePassword;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.User.ChangePassword;

public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, password) = UserBuilder.Build();

        var request = ChangePasswordRequestJsonBuilder.Build();
        request.Password = password;

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(request);

        await act.Should().NotThrowAsync();

        var encrypter = PasswordEncrypterBuilder.Build();

        user.Password.Should().Be(encrypter.Encrypt(request.NewPassword));
    }

    [Fact]
    public async Task NewPasswordEmptyError()
    {
        var (user, password) = UserBuilder.Build();

        var request = new ChangePasswordRequestJson
        {
            Password = password,
            NewPassword = string.Empty
        };

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(request);

        var error = await act.Should().ThrowAsync<ErrorOnValidationException>();

        error.Where(e =>
            e.ErrorMessages.Count == 1
            && e.ErrorMessages.Contains(ResourceMessageExceptions.PASSWORD_INVALID));

        var encrypter = PasswordEncrypterBuilder.Build();

        user.Password.Should().Be(encrypter.Encrypt(password));
    }

    [Fact]
    public async Task CurrentPasswordDoesNotMatchError()
    {
        var (user, password) = UserBuilder.Build();

        var request = ChangePasswordRequestJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(request);

        var error = await act.Should().ThrowAsync<ErrorOnValidationException>();

        error.Where(e =>
            e.ErrorMessages.Count == 1
            && e.ErrorMessages.Contains(ResourceMessageExceptions.PASSWORD_INVALID));

        var encrypter = PasswordEncrypterBuilder.Build();

        user.Password.Should().Be(encrypter.Encrypt(password));
    }

    private static ChangePasswordUseCase CreateUseCase(RecipeBook.Domain.Entities.User user)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var encrypter = PasswordEncrypterBuilder.Build();
        var updateRepo = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();

        return new ChangePasswordUseCase(
            loggedUser,
            updateRepo,
            unitOfWork, encrypter);
    }
}