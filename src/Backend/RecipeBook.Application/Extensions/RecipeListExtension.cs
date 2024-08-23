using AutoMapper;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Services.Storage;

namespace RecipeBook.Application.Extensions;

public static class RecipeListExtension
{
    public static async Task<IList<ShortRecipeResponseJson>> MapToShortRecipe(
        this IList<Recipe> recipes,
        User user,
        IMapper mapper,
        IBlobStorageService blobStorageService)
    {
        var result = recipes.Select(async recipe =>
        {
            var response = mapper.Map<ShortRecipeResponseJson>(recipe);

            if (recipe.ImageIdentifier.IsNotEmpty())
            {
                response.ImageUrl = await blobStorageService.GetFileUrl(user, recipe.ImageIdentifier);
            }

            return response;
        });

        var list = await Task.WhenAll(result);

        return list;
    }
}