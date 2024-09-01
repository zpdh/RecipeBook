using Microsoft.AspNetCore.Authentication.BearerToken;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Domain.ValueObjects;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.RefreshToken;

public class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public RefreshTokenUseCase(
        ITokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<TokensResponseJson> Execute(NewTokenRequestJson request)
    {
        var refreshToken = await _tokenRepository.Get(request.RefreshToken);

        if (refreshToken is null)
        {
            throw new RefreshTokenNotFoundException();
        }

        // Checks if token is not expired
        var refreshTokenExpirationDate = refreshToken
            .CreationDate
            .AddDays(RuleConstants.RefreshTokenExpirationInDays)
            .Date;

        if (DateTime.Compare(refreshTokenExpirationDate, DateTime.UtcNow) < 0)
        {
            throw new RefreshTokenExpiredException();
        }

        // Create new token
        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            UserId = refreshToken.UserId,
            Value = _refreshTokenGenerator.Generate()
        };

        // Save new token and return it
        await _tokenRepository.SaveRefreshToken(newRefreshToken);
        await _unitOfWork.Commit();

        return new TokensResponseJson
        {
            AccessToken = _accessTokenGenerator.Generate(refreshToken.User.UserIdentifier),
            RefreshToken = newRefreshToken.Value
        };
    }
}