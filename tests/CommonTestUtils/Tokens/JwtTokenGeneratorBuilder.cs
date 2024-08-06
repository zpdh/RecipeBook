using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Infrastructure.Security.Tokens.Generators;

namespace CommonTestUtils.Tokens;

public class JwtTokenGeneratorBuilder
{
    public static IAccessTokenGenerator Build()
    {
        return new JwtTokenGenerator(
            expirationInMinutes: 5,
            signingKey: "abcefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuv");
    }
}