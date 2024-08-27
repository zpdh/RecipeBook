using Moq;
using RecipeBook.Domain.Services.ServiceBus;

namespace CommonTestUtils.ServiceBus;

public class DeleteUserQueueBuilder
{
    public static IDeleteUserQueue Build()
    {
        var mock = new Mock<IDeleteUserQueue>();

        return mock.Object;
    }
}