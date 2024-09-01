using Bogus;
using RecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class RegisterUserRequestJsonBuilder
{
    public static RegisterUserRequestJson Build(int passwordLength = 10)
    {
        return new Faker<RegisterUserRequestJson>()
            .RuleFor(user => user.Name, faker => faker.Person.FirstName)
            .RuleFor(user => user.Email, (faker, user) => faker.Internet.Email(user.Name))
            .RuleFor(user => user.Password, faker => faker.Internet.Password(passwordLength));
    }
}