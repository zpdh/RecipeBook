using Bogus;
using CommonTestUtils.Cryptography;
using RecipeBook.Domain.Entities;

namespace CommonTestUtils.Entities;

public class UserBuilder
{
    public static (User user, string password) Build()
    {
        var encrypter = PasswordEncrypterBuilder.Build();
        var password = new Faker().Internet.Password();

        var user = new Faker<User>()
            .RuleFor(u => u.Id, () => 1)
            .RuleFor(u => u.Name, faker => faker.Person.FirstName)
            .RuleFor(u => u.Email, (faker, user) => faker.Internet.Email(user.Name))
            .RuleFor(u => u.Password, () => encrypter.Encrypt(password));
        return (user, password);
    }
}