using Microsoft.EntityFrameworkCore;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories;

namespace RecipeBook.Infrastructure.DataAccess.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly RecipeBookDbContext _context;

    public TokenRepository(RecipeBookDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> Get(string refreshToken)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Include(token => token.User)
            .FirstOrDefaultAsync(token => token.Value == refreshToken);
    }

    public async Task SaveRefreshToken(RefreshToken refreshToken)
    {
        var tokens = _context.RefreshTokens.Where(token => token.UserId == refreshToken.UserId);
        _context.RefreshTokens.RemoveRange(tokens);

        await _context.AddAsync(refreshToken);
    }
}