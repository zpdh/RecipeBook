using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RecipeBook.Infrastructure.Security.Tokens;

public abstract class JwtTokenHandler
{
    protected SymmetricSecurityKey SecurityKey(string signingKey)
    {
        var bytes = Encoding.UTF8.GetBytes(signingKey);

        return new SymmetricSecurityKey(bytes);
    }
}