using Bogus;
using RecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class GenerateRecipeRequestJsonBuilder
{
    public static GenerateRecipeRequestJson Build(int count = 5)
    {
        return new Faker<GenerateRecipeRequestJson>()
            .RuleFor(recipe => recipe.Ingredients,
                faker => faker.Make(count, () => faker.Commerce.ProductName()));
    }
}