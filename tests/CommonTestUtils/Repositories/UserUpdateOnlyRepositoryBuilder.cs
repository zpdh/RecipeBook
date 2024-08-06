using Moq;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories.User;

namespace CommonTestUtils.Repositories;

public class UserUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IUserUpdateOnlyRepository> _repository;

    public UserUpdateOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUserUpdateOnlyRepository>();
    }

    public IUserUpdateOnlyRepository Build()
    {
        return _repository.Object;
    }

    public UserUpdateOnlyRepositoryBuilder GetById(User user)
    {
        _repository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);

        return this;
    }
}