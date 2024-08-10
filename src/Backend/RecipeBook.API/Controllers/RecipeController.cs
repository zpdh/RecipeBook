using Microsoft.AspNetCore.Mvc;
using RecipeBook.API.Attributes;
using RecipeBook.Application.UseCases.Recipe.Register;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.API.Controllers;

[AuthenticatedUser]
public class RecipeController : RecipeBookBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(RegisteredRecipeResponseJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterRecipeUseCase useCase,
        [FromBody] RecipeRequestJson request)
    {
        var response = await useCase.Execute(request);

        return Created(string.Empty, response);
    }
}