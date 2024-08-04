using Microsoft.AspNetCore.Mvc;
using RecipeBook.Application.UseCases.User.Registration;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.API.Controllers;

public class UserController : RecipeBookBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(RegisterUserResponseJson), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterUserUseCase useCase,
        [FromBody] RegisterUserRequestJson request)
    {
        var result = await useCase.Execute(request);

        return Created(string.Empty, result);
    }
}