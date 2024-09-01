using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Domain.Extensions;

namespace RecipeBook.API.Controllers;

[ApiController]
[Route("[controller]")]
public class RecipeBookBaseController : ControllerBase
{
    protected static bool IsNotAuthenticated(AuthenticateResult result)
    {
        return result.Succeeded.IsFalse() ||
               result.Principal is null ||
               result.Principal.Identities.Any(id => id.IsAuthenticated).IsFalse();
    }
}