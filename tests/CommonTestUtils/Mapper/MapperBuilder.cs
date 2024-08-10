using AutoMapper;
using CommonTestUtils.Cryptography;
using RecipeBook.Application.Services.AutoMapper;

namespace CommonTestUtils.Mapper;

public class MapperBuilder
{
    public static IMapper Build()
    {
        var encoder = IdEncoderBuilder.Build();
        
        return new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapping(encoder));
        }).CreateMapper();
    }
}