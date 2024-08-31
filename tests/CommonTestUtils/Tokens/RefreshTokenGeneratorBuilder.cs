using RecipeBook.Infrastructure.Security.Tokens.Generators;

namespace CommonTestUtils.Tokens;

public class RefreshTokenGeneratorBuilder
{
    public static RefreshTokenGenerator Build()
    {
        return new RefreshTokenGenerator();
    }
}