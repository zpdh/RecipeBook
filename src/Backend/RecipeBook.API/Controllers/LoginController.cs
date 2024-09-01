using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Application.UseCases.Login.ExecuteLogin;
using RecipeBook.Application.UseCases.Login.ExternalLogin;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.API.Controllers;

public class LoginController : RecipeBookBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(RegisterUserResponseJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromServices] IExecuteLoginUseCase useCase,
        [FromBody] LoginRequestJson request)
    {
        var response = await useCase.Execute(request);

        return Ok(response);
    }

    [HttpGet]
    [Route("google")]
    public async Task<IActionResult> LoginWithGoogle(
        [FromServices] IExternalLoginUseCase useCase,
        string returnUrl)
    {
        var result = await Request.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (IsNotAuthenticated(result))
        {
            return Challenge(GoogleDefaults.AuthenticationScheme);
        }

        var claims = result.Principal!.Identities.First().Claims.ToArray();

        var name = claims.First(claim => claim.Type.Equals(ClaimTypes.Name)).Value;
        var email = claims.First(claim => claim.Type.Equals(ClaimTypes.Email)).Value;

        var token = await useCase.Execute(name, email);

        return Redirect($"{returnUrl}/{token}");
    }
}