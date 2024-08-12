using Bogus;
using RecipeBook.Communication.Enums;
using RecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class RecipeFilterRequestJsonBuilder
{
    public static RecipeFilterRequestJson Build()
    {
        return new Faker<RecipeFilterRequestJson>()
            .RuleFor(r => r.RecipeTitleOrIngredient, faker => faker.Random.Word())
            .RuleFor(r => r.CookingTimes, faker => faker.Make(3, faker.PickRandom<CookingTime>))
            .RuleFor(r => r.Difficulties, faker => faker.Make(3, faker.PickRandom<Difficulty>))
            .RuleFor(r => r.DishTypes, faker => faker.Make(3, faker.PickRandom<DishType>));
    }
}