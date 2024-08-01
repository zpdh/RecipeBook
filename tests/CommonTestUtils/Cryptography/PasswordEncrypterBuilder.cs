using RecipeBook.Application.Services.Cryptography;

namespace CommonTestUtils.Cryptography;

public class PasswordEncrypterBuilder
{
    public static PasswordEncrypter Build() => new PasswordEncrypter();
}