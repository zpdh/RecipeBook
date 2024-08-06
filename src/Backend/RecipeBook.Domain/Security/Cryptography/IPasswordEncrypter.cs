namespace RecipeBook.Domain.Security.Cryptography;

public interface IPasswordEncrypter
{
    public string Encrypt(string password);
}