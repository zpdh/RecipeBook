using Microsoft.EntityFrameworkCore;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories.User;

namespace RecipeBook.Infrastructure.DataAccess.Repositories;

public class UserRepository :
    IUserReadOnlyRepository,
    IUserWriteOnlyRepository,
    IUserUpdateOnlyRepository,
    IUserDeleteOnlyRepository
{
    private readonly RecipeBookDbContext _context;

    public UserRepository(RecipeBookDbContext context) => _context = context;

    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> ActiveUserWithEmailExists(string email)
    {
        return await _context
            .Users
            .AnyAsync(u => u.Email.Equals(email) && u.IsActive);
    }

    public async Task<bool> ActiveUserWithIdentifierExists(Guid userIdentifier)
    {
        return await _context
            .Users
            .AnyAsync(u => u.UserIdentifier.Equals(userIdentifier) && u.IsActive);
    }

    public async Task<User?> GetByEmailAndPassword(string email, string password)
    {
        var user = await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.Equals(email) && u.Password.Equals(password));

        return user;
    }

    public async Task<User> GetById(long id)
    {
        var user = await _context.Users
            .FirstAsync(u => u.Id == id);

        return user;
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public async Task Delete(Guid userIdentifier)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);

        if (user is null) return;

        var recipes = _context.Recipes
            .Where(recipe => recipe.UserId == user.Id);

        _context.Recipes.RemoveRange(recipes);
        _context.Users.Remove(user);
    }
}