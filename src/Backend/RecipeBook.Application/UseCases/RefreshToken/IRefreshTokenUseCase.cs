using Microsoft.AspNetCore.Authentication.BearerToken;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.Application.UseCases.RefreshToken;

public interface IRefreshTokenUseCase
{
    public Task<TokensResponseJson> Execute(NewTokenRequestJson request);
}