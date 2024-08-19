using Microsoft.AspNetCore.Mvc;
using RecipeBook.API.Attributes;
using RecipeBook.API.Binders;
using RecipeBook.Application.UseCases.Recipe.Delete;
using RecipeBook.Application.UseCases.Recipe.Filter;
using RecipeBook.Application.UseCases.Recipe.Generate;
using RecipeBook.Application.UseCases.Recipe.GetById;
using RecipeBook.Application.UseCases.Recipe.Register;
using RecipeBook.Application.UseCases.Recipe.Update;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;

namespace RecipeBook.API.Controllers;

[AuthenticatedUser]
public class RecipeController : RecipeBookBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(RegisteredRecipeResponseJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterRecipeUseCase useCase,
        [FromBody] RecipeRequestJson request)
    {
        var response = await useCase.Execute(request);

        return Created(string.Empty, response);
    }

    [HttpPost("filter")]
    [ProducesResponseType(typeof(RecipesResponseJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Filter(
        [FromServices] IFilterRecipeUseCase useCase,
        [FromBody] RecipeFilterRequestJson request)
    {
        var response = await useCase.Execute(request);

        if (response.Recipes.Any())
        {
            return Ok(response);
        }

        return NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetRecipeByIdUseCase useCase,
        [FromRoute] [ModelBinder(typeof(IdBinder))]
        long id)
    {
        var response = await useCase.Execute(id);

        return Ok(response);
    }

    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteRecipeUseCase useCase,
        [FromRoute] [ModelBinder(typeof(IdBinder))]
        long id)
    {
        await useCase.Execute(id);

        return NoContent();
    }

    [HttpPut]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateRecipeUseCase useCase,
        [FromRoute] [ModelBinder(typeof(IdBinder))]
        long id,
        [FromBody] RecipeRequestJson request)
    {
        await useCase.Execute(id, request);

        return NoContent();
    }

    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate(
        [FromServices] IGenerateRecipeUseCase useCase,
        [FromBody] GenerateRecipeRequestJson request)
    {
        /*
         * There are no integration tests for this class since
         * it would cost OpenAI tokens to run.
         */
        
        var response = await useCase.Execute(request);

        return Ok(response);
    }
}