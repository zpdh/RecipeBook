using AutoMapper;
using RecipeBook.Application.Services.AutoMapper;

namespace CommonTestUtils.Mapper;

public class MapperBuilder
{
    public static IMapper Build()
    {
        return new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapping());
        }).CreateMapper();
    }
}