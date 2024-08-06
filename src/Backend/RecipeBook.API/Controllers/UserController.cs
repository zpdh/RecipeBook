using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.API.Attributes;
using RecipeBook.Application.UseCases.User.Profile;
using RecipeBook.Application.UseCases.User.Registration;
using RecipeBook.Application.UseCases.User.Update;
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

    [HttpGet]
    [AuthenticatedUser]
    [ProducesResponseType(typeof(UserProfileResponseJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserProfile(
        [FromServices] IGetUserProfileUseCase useCase)
    {
        var result = await useCase.Execute();

        return Ok(result);
    }

    [HttpPut]
    [AuthenticatedUser]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(
        [FromServices] IUpdateUserUseCase useCase,
        [FromBody] UpdateUserRequestJson request)
    {
        await useCase.Execute(request);
        
        return NoContent();
    }
}