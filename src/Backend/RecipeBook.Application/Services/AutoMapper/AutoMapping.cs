using AutoMapper;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Enums;
using Sqids;

namespace RecipeBook.Application.Services.AutoMapper;

public class AutoMapping : Profile
{
    private readonly SqidsEncoder<long> _idEncoder;

    public AutoMapping(SqidsEncoder<long> idEncoder)
    {
        _idEncoder = idEncoder;
        RequestToDomain();
        DomainToResponse();
    }

    private void RequestToDomain()
    {
        CreateMap<RegisterUserRequestJson, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<RecipeRequestJson, Recipe>()
            .ForMember(dest => dest.Instructions, opt => opt.Ignore())
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients.Distinct()))
            .ForMember(dest => dest.DishTypes, opt => opt.MapFrom(src => src.DishTypes.Distinct()));

        CreateMap<string, Ingredient>()
            .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src));

        CreateMap<Communication.Enums.DishType, Domain.Entities.DishType>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

        CreateMap<InstructionRequestJson, Instruction>();
    }

    private void DomainToResponse()
    {
        CreateMap<User, UserProfileResponseJson>();

        CreateMap<Recipe, RegisteredRecipeResponseJson>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => _idEncoder.Encode(src.Id)));

        CreateMap<Recipe, ShortRecipeResponseJson>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => _idEncoder.Encode(src.Id)))
            .ForMember(dest => dest.IngredientAmount, config => config.MapFrom(src => src.Ingredients.Count));

        CreateMap<Recipe, RecipeResponseJson>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => _idEncoder.Encode(src.Id)))
            .ForMember(dest => dest.DishTypes, config => config.MapFrom(src => src.DishTypes.Select(d => d.Type)));

        CreateMap<Ingredient, IngredientResponseJson>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => _idEncoder.Encode(src.Id)));

        CreateMap<Instruction, InstructionResponseJson>()
            .ForMember(dest => dest.Id, config => config.MapFrom(src => _idEncoder.Encode(src.Id)));
    }
}