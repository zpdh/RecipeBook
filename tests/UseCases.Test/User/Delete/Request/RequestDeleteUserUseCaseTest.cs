using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Repositories;
using CommonTestUtils.ServiceBus;
using FluentAssertions;
using RecipeBook.Application.UseCases.User.Delete.Request;

namespace UseCases.Tests.User.Delete.Request;

public class RequestDeleteUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute();

        await act.Should().NotThrowAsync();
        user.IsActive.Should().BeFalse();
    }

    // No error tests for this use case

    private static RequestDeleteUserUseCase CreateUseCase(RecipeBook.Domain.Entities.User user)
    {
        var queue = DeleteUserQueueBuilder.Build();
        var repo = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new RequestDeleteUserUseCase(repo, unitOfWork, loggedUser, queue);
    }
}