using Moq;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Services.LoggedUser;

namespace CommonTestUtils.LoggedUser;

public class LoggedUserBuilder
{
    public static ILoggedUser Build(User user)
    {
        var mock = new Mock<ILoggedUser>();

        mock.Setup(u => u.User()).ReturnsAsync(user);

        return mock.Object;
    }
}