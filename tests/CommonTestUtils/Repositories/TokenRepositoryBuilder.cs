using Moq;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories;

namespace CommonTestUtils.Repositories;

public class TokenRepositoryBuilder
{
    private readonly Mock<ITokenRepository> _mock;

    public TokenRepositoryBuilder()
    {
        _mock = new Mock<ITokenRepository>();
    }

    public ITokenRepository Build()
    {
        return _mock.Object;
    }

    public TokenRepositoryBuilder Get(RefreshToken? refreshToken)
    {
        if (refreshToken is not null)
        {
            _mock.Setup(tokenRepo => tokenRepo.Get(refreshToken.Value)).ReturnsAsync(refreshToken);
        }

        return this;
    }
}