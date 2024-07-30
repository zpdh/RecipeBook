using AutoMapper;
using RecipeBook.Communication.Requests;
using RecipeBook.Domain.Entities;

namespace RecipeBook.Application.Services.AutoMapper;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        RequestToDomain();
    }

    private void RequestToDomain()
    {
        CreateMap<RegisterUserRequestJson, User>()
            .ForMember(destination => destination.Password, option => option.Ignore());
    }
}