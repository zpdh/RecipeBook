using Bogus;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Enums;
using DishType = RecipeBook.Domain.Enums.DishType;

namespace CommonTestUtils.Entities;

public class RecipeBuilder
{
    public static IList<Recipe> Collection(User user, uint count = 2)
    {
        var list = new List<Recipe>();

        if (count is 0) count = 1;

        for (var i = 0; i < count; i++)
        {
            var fakeRecipe = Build(user);

            fakeRecipe.Id = i + 1;

            list.Add(fakeRecipe);
        }

        return list;
    }

    public static Recipe Build(User user)
    {
        return new Faker<Recipe>()
            .RuleFor(r => r.Id, () => 1)
            .RuleFor(r => r.Title, faker => faker.Random.Word())
            .RuleFor(r => r.Difficulty, faker => faker.PickRandom<Difficulty>())
            .RuleFor(r => r.CookingTime, faker => faker.PickRandom<CookingTime>())
            .RuleFor(r => r.ImageIdentifier, _ => $"{Guid.NewGuid()}.png")
            .RuleFor(r => r.Ingredients, faker => faker.Make(1, () => new Ingredient
            {
                Id = 1,
                Item = faker.Commerce.ProductName()
            })).RuleFor(r => r.Instructions, faker => faker.Make(1, () => new Instruction
            {
                Id = 1,
                Step = 1,
                Text = faker.Lorem.Paragraph()
            })).RuleFor(r => r.DishTypes, faker => faker.Make(1, () => new RecipeBook.Domain.Entities.DishType
            {
                Id = 1,
                Type = faker.PickRandom<DishType>()
            })).RuleFor(r => r.UserId, () => user.Id);
    }
}