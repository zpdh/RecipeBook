using RecipeBook.Domain.Security.Tokens;

namespace RecipeBook.Infrastructure.Security.Tokens.Generators;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string Generate()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}