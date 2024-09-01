using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Infrastructure.DataAccess;

namespace RecipeBook.Infrastructure.Services.LoggedUser;

public class LoggedUser : ILoggedUser
{
    private readonly RecipeBookDbContext _context;
    private readonly ITokenProvider _tokenProvider;

    public LoggedUser(RecipeBookDbContext context, ITokenProvider tokenProvider)
    {
        _context = context;
        _tokenProvider = tokenProvider;
    }

    public async Task<User> User()
    {
        var token = _tokenProvider.Value();

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

        var identifier = Guid.Parse(
            jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);

        return await _context.Users
            .AsNoTracking()
            .FirstAsync(u => u.IsActive && u.UserIdentifier == identifier);
    }
}