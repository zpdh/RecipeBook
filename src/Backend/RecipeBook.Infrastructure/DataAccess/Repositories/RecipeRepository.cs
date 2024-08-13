using Microsoft.EntityFrameworkCore;
using RecipeBook.Domain.DTOs;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories.Recipe;

namespace RecipeBook.Infrastructure.DataAccess.Repositories;

public class RecipeRepository : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository
{
    private readonly RecipeBookDbContext _context;

    public RecipeRepository(RecipeBookDbContext context)
    {
        _context = context;
    }

    public async Task Add(Recipe recipe)
    {
        await _context.Recipes
            .AddAsync(recipe);
    }

    public async Task<IList<Recipe>> Filter(User user, RecipeFiltersDto filters)
    {
        var query = _context.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .Where(recipe => recipe.IsActive && recipe.UserId == user.Id);

        query = ApplyFilters(filters, query);

        return await query.ToListAsync();
    }

    public async Task<Recipe?> GetById(User user, long recipeId)
    {
        return await _context.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .Include(recipe => recipe.Instructions)
            .Include(recipe => recipe.DishTypes)
            .FirstOrDefaultAsync(recipe => recipe.IsActive && recipe.Id == recipeId && recipe.UserId == user.Id);
    }

    private static IQueryable<Recipe> ApplyFilters(RecipeFiltersDto filters, IQueryable<Recipe> query)
    {
        if (filters.Difficulties.Any())
        {
            query = query.Where(recipe =>
                recipe.Difficulty.HasValue
                && filters.Difficulties.Contains(recipe.Difficulty.Value));
        }

        if (filters.CookingTimes.Any())
        {
            query = query.Where(recipe =>
                recipe.CookingTime.HasValue
                && filters.CookingTimes.Contains(recipe.CookingTime.Value));
        }

        if (filters.DishTypes.Any())
        {
            query = query.Where(recipe =>
                recipe.DishTypes.Any(dishType => filters.DishTypes.Contains(dishType.Type)));
        }

        if (filters.RecipeTitleOrIngredient.IsNotEmpty())
        {
            query = query.Where(recipe =>
                recipe.Title.Contains(filters.RecipeTitleOrIngredient)
                || recipe.Ingredients.Any(ingredient => ingredient.Item.Contains(filters.RecipeTitleOrIngredient)));
        }

        return query;
    }
}