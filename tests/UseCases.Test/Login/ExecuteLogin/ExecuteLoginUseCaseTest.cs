using CommonTestUtils.Cryptography;
using CommonTestUtils.Entities;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Application.UseCases.Login.ExecuteLogin;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;
using RecipeBook.Infrastructure.Security.Tokens.Generators;

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
        result.Tokens.Should().NotBeNull();

        result.Name.Should().NotBeNullOrWhiteSpace().And.Be(user.Name);
        result.Tokens.AccessToken.Should().NotBeNullOrWhiteSpace();
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
        var tokenGenerator = JwtTokenGeneratorBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var tokenRepo = new TokenRepositoryBuilder().Build();
        var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();


        if (user is not null)
        {
            readRepoBuilder.GetByEmail(user);
        }

        return new ExecuteLoginUseCase(
            readRepoBuilder.Build(),
            PasswordEncrypterBuilder.Build(),
            tokenGenerator,
            refreshTokenGenerator,
            tokenRepo,
            unitOfWork);
    }
}