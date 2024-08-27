using Moq;
using RecipeBook.Domain.Repositories.User;

namespace CommonTestUtils.Repositories;

public class UserDeleteOnlyRepositoryBuilder
{
    public static IUserDeleteOnlyRepository Build()
    {
        var mock = new Mock<IUserDeleteOnlyRepository>();

        return mock.Object;
    }
}