using Bogus;
using Microsoft.AspNetCore.Http;
using RecipeBook.Communication.Enums;
using RecipeBook.Communication.Requests;

namespace CommonTestUtils.Requests;

public class RegisterRecipeFormDataRequestBuilder
{
    public static RegisterRecipeFormDataRequest Build(IFormFile? formFile = null)
    {
        var step = 1;

        return new Faker<RegisterRecipeFormDataRequest>()
            .RuleFor(r => r.Image, _ => formFile)
            .RuleFor(r => r.Title, faker => faker.Lorem.Word())
            .RuleFor(r => r.CookingTime, faker => faker.PickRandom<CookingTime>())
            .RuleFor(r => r.Difficulty, faker => faker.PickRandom<Difficulty>())
            .RuleFor(r => r.Ingredients, faker => faker.Make(3, () => faker.Commerce.ProductName()))
            .RuleFor(r => r.DishTypes, faker => faker.Make(3, faker.PickRandom<DishType>))
            .RuleFor(r => r.Instructions, faker => faker.Make(3, () => new InstructionRequestJson
            {
                Text = faker.Lorem.Paragraph(),
                Step = step++
            }));
    }
}