using CommonTestUtils.Entities;
using CommonTestUtils.Repositories;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Application.UseCases.RefreshToken;
using RecipeBook.Communication.Requests;
using RecipeBook.Domain.ValueObjects;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.RefreshToken;

public class RefreshTokenUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var refreshToken = RefreshTokenBuilder.Build(user);

        var useCase = CreateUseCase(refreshToken);

        var result = await useCase.Execute(new NewTokenRequestJson
        {
            RefreshToken = refreshToken.Value
        });

        result.Should().NotBeNull();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task NotFoundError()
    {
        var (user, _) = UserBuilder.Build();
        var refreshToken = RefreshTokenBuilder.Build(user);

        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(new NewTokenRequestJson
        {
            RefreshToken = refreshToken.Value
        });

        (await act.Should().ThrowAsync<RefreshTokenNotFoundException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessageExceptions.REFRESH_TOKEN_NOT_FOUND));
    }

    [Fact]
    public async Task ExpiredTokenError()
    {
        var (user, _) = UserBuilder.Build();
        var refreshToken = RefreshTokenBuilder.Build(user);
        // Removing the expiration + 1 days from creation date
        refreshToken.CreationDate = DateTime.UtcNow.AddDays(-(RuleConstants.RefreshTokenExpirationInDays + 1));

        var useCase = CreateUseCase(refreshToken);

        var act = async () => await useCase.Execute(new NewTokenRequestJson
        {
            RefreshToken = refreshToken.Value
        });
        
        (await act.Should().ThrowAsync<RefreshTokenExpiredException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessageExceptions.REFRESH_TOKEN_EXPIRED));
    }


    private static RefreshTokenUseCase CreateUseCase(RecipeBook.Domain.Entities.RefreshToken? refreshToken = null)
    {
        var tokenRepo = new TokenRepositoryBuilder().Get(refreshToken).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var tokenGenerator = JwtTokenGeneratorBuilder.Build();
        var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();

        return new RefreshTokenUseCase(tokenRepo, unitOfWork, tokenGenerator, refreshTokenGenerator);
    }
}