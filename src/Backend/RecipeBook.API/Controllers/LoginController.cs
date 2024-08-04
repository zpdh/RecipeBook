using Microsoft.AspNetCore.Mvc;
using RecipeBook.Application.UseCases.Login.ExecuteLogin;
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
}