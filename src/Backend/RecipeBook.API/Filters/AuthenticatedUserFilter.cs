using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.API.Filters;

public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
{
    private readonly IAccessTokenValidator _accessTokenValidator;
    private readonly IUserReadOnlyRepository _readOnlyRepository;

    public AuthenticatedUserFilter(
        IAccessTokenValidator accessTokenValidator,
        IUserReadOnlyRepository readOnlyRepository)
    {
        _accessTokenValidator = accessTokenValidator;
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenOnRequest(context);

            var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);

            var exists = await _readOnlyRepository.ActiveUserWithIdentifierExists(userIdentifier);

            if (exists.IsFalse())
            {
                throw new RecipeBookAuthenticationException(ResourceMessageExceptions.USER_WITHOUT_PERMISSION);
            }
        }
        catch (RecipeBookException e)
        {
            context.Result = new UnauthorizedObjectResult(new ErrorResponseJson(e.Message));
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = new UnauthorizedObjectResult(new ErrorResponseJson("Token is expired")
            {
                TokenIsExpired = true
            });
        }
        catch
        {
            context.Result = new UnauthorizedObjectResult(
                new ErrorResponseJson(ResourceMessageExceptions.USER_WITHOUT_PERMISSION));
        }
    }

    private static string TokenOnRequest(AuthorizationFilterContext context)
    {
        var authentication = context.HttpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authentication))
        {
            throw new RecipeBookAuthenticationException(ResourceMessageExceptions.NO_TOKEN);
        }

        //.. = Range operator; Splits the array at given position and returns everything before/after said position
        var token = authentication["Bearer ".Length..].Trim();

        return token;
    }
}