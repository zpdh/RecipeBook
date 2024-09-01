using Microsoft.AspNetCore.Http;

namespace RecipeBook.Application.UseCases.Recipe.Image;

public interface IUpdateImageUseCase
{
    Task Execute(long id, IFormFile file);
}