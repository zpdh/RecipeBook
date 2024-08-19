using Bogus;
using RecipeBook.Domain.DTOs;
using RecipeBook.Domain.Enums;

namespace CommonTestUtils.DTOs;

public class GeneratedRecipeDtoBuilder
{
    public static GeneratedRecipeDto Build()
    {
        return new Faker<GeneratedRecipeDto>()
            .RuleFor(dto => dto.Title, faker => faker.Company.CompanyName())
            .RuleFor(dto => dto.CookingTime, faker => faker.PickRandom<CookingTime>())
            .RuleFor(dto => dto.Ingredients, faker => faker.Make(1, () => faker.Commerce.ProductName()))
            .RuleFor(dto => dto.Instructions, faker => faker.Make(1, () => new GeneratedInstructionDto
            {
                Step = 1,
                Text = faker.Lorem.Paragraph()
            }));
    }
}