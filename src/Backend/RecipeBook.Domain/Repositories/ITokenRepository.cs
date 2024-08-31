using RecipeBook.Domain.Entities;

namespace RecipeBook.Domain.Repositories;

public interface ITokenRepository
{
    Task<RefreshToken?> Get(string refreshToken);
    Task SaveRefreshToken(RefreshToken refreshToken);
}