using Microsoft.EntityFrameworkCore;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories.User;

namespace RecipeBook.Infrastructure.DataAccess.Repositories;

public class UserRepository : IUserReadOnlyRepository, IUserWriteOnlyRepository
{
    private readonly RecipeBookDbContext _context;

    public UserRepository(RecipeBookDbContext context) => _context = context;

    public async Task Add(User user) => await _context.Users.AddAsync(user);

    public async Task<bool> ActiveUserWithEmailExists(string email) =>
        await _context.Users.AnyAsync(u => u.Email.Equals(email) && u.IsActive);
}