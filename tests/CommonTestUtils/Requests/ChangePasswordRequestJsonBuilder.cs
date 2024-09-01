using Bogus;
using RecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class ChangePasswordRequestJsonBuilder
{
    public static ChangePasswordRequestJson Build(int passwordLength = 10)
    {
        return new Faker<ChangePasswordRequestJson>()
            .RuleFor(req => req.Password, faker => faker.Internet.Password())
            .RuleFor(req => req.NewPassword, faker => faker.Internet.Password(passwordLength));

    }
}