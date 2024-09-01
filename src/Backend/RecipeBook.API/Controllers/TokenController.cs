using Microsoft.AspNetCore.Mvc;
using RecipeBook.Application.UseCases.RefreshToken;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.API.Controllers;

public class TokenController : RecipeBookBaseController
{
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokensResponseJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken(
        [FromServices] IRefreshTokenUseCase useCase,
        [FromBody] NewTokenRequestJson request)
    {
        var response = await useCase.Execute(request);

        return Ok(response);
    }
}