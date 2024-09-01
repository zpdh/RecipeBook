using Bogus;
using RecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class UpdateUserRequestJsonBuilder
{
    public static UpdateUserRequestJson Build()
    {
        return new Faker<UpdateUserRequestJson>()
            .RuleFor(u => u.Name, faker => faker.Name.FirstName())
            .RuleFor(u => u.Email, (faker, user) => faker.Internet.Email(user.Name));
    }
}