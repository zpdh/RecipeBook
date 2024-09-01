using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.User.Update;
using RecipeBook.Communication.Requests;
using RecipeBook.Domain.Extensions;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.User.Update;

public class UpdateUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var request = UpdateUserRequestJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(request);

        await act.Should().NotThrowAsync();

        user.Name.Should().Be(request.Name);
        user.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task NameEmptyError()
    {
        var (user, _) = UserBuilder.Build();

        var request = UpdateUserRequestJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(request);

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(e =>
            e.GetErrorMessages().Count == 1
            && e.GetErrorMessages().Contains(ResourceMessageExceptions.NAME_EMPTY));

        user.Name.Should().NotBe(request.Name);
        user.Email.Should().NotBe(request.Email);
    }

    [Fact]
    public async Task EmailAlreadyRegisteredError()
    {
        var (user, _) = UserBuilder.Build();

        var request = UpdateUserRequestJsonBuilder.Build();

        var useCase = CreateUseCase(user, request.Email);

        var act = async () => await useCase.Execute(request);

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(e =>
            e.GetErrorMessages().Count == 1
            && e.GetErrorMessages().Contains(ResourceMessageExceptions.EMAIL_ALREADY_REGISTERED));

        user.Name.Should().NotBe(request.Name);
        user.Email.Should().NotBe(request.Email);
    }

    private static UpdateUserUseCase CreateUseCase(
        RecipeBook.Domain.Entities.User user,
        string? email = null)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var readRepo = new UserReadOnlyRepositoryBuilder();
        var updateRepo = new UserUpdateOnlyRepositoryBuilder()
            .GetById(user)
            .Build();

        if (email.IsNotEmpty())
        {
            readRepo.ActiveUserWithEmailExists(email);
        }

        return new UpdateUserUseCase(
            loggedUser,
            updateRepo,
            readRepo.Build(),
            unitOfWork);
    }
}