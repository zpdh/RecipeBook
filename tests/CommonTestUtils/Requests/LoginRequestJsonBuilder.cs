using Bogus;
using RecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class LoginRequestJsonBuilder
{
    public static LoginRequestJson Build()
    {
        return new Faker<LoginRequestJson>()
            .RuleFor(u => u.Email, faker => faker.Internet.Email())
            .RuleFor(u => u.Password, faker => faker.Internet.Password());
    }
}