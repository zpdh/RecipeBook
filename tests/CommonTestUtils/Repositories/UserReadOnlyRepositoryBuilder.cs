using Moq;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories.User;

namespace CommonTestUtils.Repositories;

public class UserReadOnlyRepositoryBuilder
{
    private readonly Mock<IUserReadOnlyRepository> _repository;

    public UserReadOnlyRepositoryBuilder() => _repository = new Mock<IUserReadOnlyRepository>();


    public IUserReadOnlyRepository Build()
    {
        return _repository.Object;
    }

    public void ActiveUserWithEmailExists(string email)
    {
        _repository
            .Setup(repository => repository.ActiveUserWithEmailExists(email))
            .ReturnsAsync(true);
    }

    public void GetByEmail(User user)
    {
        _repository
            .Setup(repository => repository.GetByEmail(user.Email))
            .ReturnsAsync(user);
    }
}