using RecipeBook.Domain.Security.Cryptography;
using RecipeBook.Infrastructure.Security.Cryptography;

namespace CommonTestUtils.Cryptography;

public class PasswordEncrypterBuilder
{
    public static IPasswordEncrypter Build()
    {
        return new BCryptHasher();
    }
}