using Bogus;
using RecipeBook.Communication.Enums;
using RecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class RecipeRequestJsonBuilder
{
    public static RecipeRequestJson Build()
    {
        var count = 0;
        return new Faker<RecipeRequestJson>()
            .RuleFor(recipe => recipe.Title, faker => faker.Lorem.Word())
            .RuleFor(recipe => recipe.CookingTime, faker => faker.PickRandom<CookingTime>())
            .RuleFor(recipe => recipe.Difficulty, faker => faker.PickRandom<Difficulty>())
            .RuleFor(recipe => recipe.Ingredients, faker => faker.Make(3, () => faker.Commerce.ProductName()))
            .RuleFor(recipe => recipe.Instructions, faker => faker.Make(3, () => new InstructionRequestJson
            {
                Text = faker.Lorem.Paragraph(),
                Step = ++count
            }))
            .RuleFor(recipe => recipe.DishTypes, faker => faker.Make(3, faker.PickRandom<DishType>));
    }
}