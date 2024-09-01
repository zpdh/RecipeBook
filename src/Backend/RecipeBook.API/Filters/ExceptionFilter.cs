using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecipeBook.Communication.Responses;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is RecipeBookException exception)
        {
            HandleException(context, exception);
        }
        else
        {
            ThrowUnknownException(context);
        }
    }

    private static void HandleException(ExceptionContext context, RecipeBookException exception)
    {
        context.HttpContext.Response.StatusCode = (int)exception.GetStatusCode();
        context.Result = new ObjectResult(new ErrorResponseJson(exception.GetErrorMessages()));
    }


    private static void ThrowUnknownException(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new ErrorResponseJson(ResourceMessageExceptions.UNKNOWN_ERROR));
    }
}