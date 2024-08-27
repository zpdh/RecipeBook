using CommonTestUtils.Blob;
using CommonTestUtils.Entities;
using CommonTestUtils.Repositories;
using FluentAssertions;
using RecipeBook.Application.UseCases.User.Delete.Delete;

namespace UseCases.Tests.User.Delete.Delete;

public class DeleteUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(user.UserIdentifier);

        await act.Should().NotThrowAsync();
    }

    // No error tests for this use case.

    private static DeleteUserUseCase CreateUseCase()
    {
        var storage = new BlobStorageServiceBuilder().Build();
        var repo = UserDeleteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new DeleteUserUseCase(repo, unitOfWork, storage);
    }
}