using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Repositories.Recipe;

namespace RecipeBook.Infrastructure.DataAccess.Repositories;

public class RecipeRepository : IRecipeWriteOnlyRepository
{
    private readonly RecipeBookDbContext _context;

    public RecipeRepository(RecipeBookDbContext context)
    {
        _context = context;
    }

    public async Task Add(Recipe recipe)
    {
        await _context.AddAsync(recipe);
    }
}