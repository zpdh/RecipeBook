using CommonTestUtils.Entities;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.Services.Cryptography;
using RecipeBook.Application.UseCases.Login.ExecuteLogin;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.Login.ExecuteLogin;

public class ExecuteLoginUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, password) = UserBuilder.Build();
        
        var useCase = InstanceUseCase(user);

        var result = await useCase.Execute(new LoginRequestJson
        {
            Email = user.Email,
            Password = password
        });

        result.Should().NotBeNull();

        result.Name.Should().NotBeNullOrWhiteSpace().And.Be(user.Name);
    }

    [Fact]
    public async Task InvalidInfoError()
    {
        var useCase = InstanceUseCase();
        var request = LoginRequestJsonBuilder.Build();

        var act = async () => await useCase.Execute(request);

        await act
            .Should()
            .ThrowAsync<InvalidLoginException>()
            .Where(e => e.Message.Equals(ResourceMessageExceptions.EMAIL_OR_PASSWORD_INVALID));
    }

    private static ExecuteLoginUseCase InstanceUseCase(RecipeBook.Domain.Entities.User? user = null)
    {
        var readRepoBuilder = new UserReadOnlyRepositoryBuilder();

        if (user is not null)
        {
            readRepoBuilder.GetByEmailAndPassword(user);
        }

        return new ExecuteLoginUseCase(readRepoBuilder.Build(), new PasswordEncrypter());
    }
}