using System.Security.Cryptography;
using System.Text;
using RecipeBook.Domain.Security.Cryptography;

namespace RecipeBook.Infrastructure.Security.Cryptography;

public class Sha512Encrypter : IPasswordEncrypter
{
    private readonly string _additionalKey;

    public Sha512Encrypter(string additionalKey = "123")
    {
        _additionalKey = additionalKey;
    }


    public string Encrypt(string password)
    {
        var newPassword = $"{password}{_additionalKey}";

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