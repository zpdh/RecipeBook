using RecipeBook.Domain.Security.Tokens;

namespace RecipeBook.API.Token;

public class HttpContextTokenValue : ITokenProvider
{
    private readonly IHttpContextAccessor _accessor;

    public HttpContextTokenValue(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Value()
    {
        var authentication = _accessor.HttpContext!.Request.Headers.Authorization.ToString();

        return authentication["Bearer ".Length..].Trim();
    }
}