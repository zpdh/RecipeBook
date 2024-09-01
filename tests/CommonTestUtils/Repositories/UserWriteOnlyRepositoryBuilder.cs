using Moq;
using RecipeBook.Domain.Repositories.User;

namespace CommonTestUtils.Repositories;

public class UserWriteOnlyRepositoryBuilder
{
    public static IUserWriteOnlyRepository Build()
    {
        var mock = new Mock<IUserWriteOnlyRepository>();

        return mock.Object;
    }
}