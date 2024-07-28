using Microsoft.AspNetCore.Mvc;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(RegisterUserResponseJson), StatusCodes.Status201Created)]
    public IActionResult Register(RegisterUserRequestJson user)
    {
        return Created();
    }
}