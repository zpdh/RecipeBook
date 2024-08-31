using Bogus;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.ValueObjects;

namespace CommonTestUtils.Entities;

public class RefreshTokenBuilder
{
    public static RefreshToken Build(User user)
    {
        return new Faker<RefreshToken>()
            .RuleFor(t => t.Id, () => 1)
            .RuleFor(t => t.CreationDate, faker => faker.Date.Soon(RuleConstants.RefreshTokenExpirationInDays))
            .RuleFor(t => t.Value, () => Guid.NewGuid().ToString())
            .RuleFor(t => t.UserId, () => user.Id)
            .RuleFor(t => t.User, () => user);
    }
}