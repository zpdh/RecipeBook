using System.Security.Cryptography;
using System.Text;

namespace RecipeBook.Application.Services.Cryptography;

public class PasswordEncrypter
{
    private readonly string PassKey;
    public PasswordEncrypter(string passKey = "123")
    {
        PassKey = passKey;
    }
    public string Encrypt(string password)
    {
        var newPassword = $"{password}{PassKey}";

        var bytes = Encoding.UTF8.GetBytes(newPassword);
        var hashBytes = SHA512.HashData(bytes);

        return BytesToString(hashBytes);
    }

    private static string BytesToString(byte[] bytes)
    {
        var stringBuilder = new StringBuilder();

        foreach (var bite in bytes)
        {
            var hex = bite.ToString("x2");
            stringBuilder.Append(hex);
        }

        return stringBuilder.ToString();
    }
}