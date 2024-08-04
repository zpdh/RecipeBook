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
        if (context.Exception is RecipeBookException)
        {
            HandleException(context);
        }
        else
        {
            ThrowUnknownException(context);
        }
    }

    private static void HandleException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ErrorOnValidationException e:
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new BadRequestObjectResult(new ErrorResponseJson(e.ErrorMessages));
                break;

            case InvalidLoginException e:
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new UnauthorizedObjectResult(new ErrorResponseJson(e.Message));
                break;
        }
    }

    private static void ThrowUnknownException(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Result = new ObjectResult(new ErrorResponseJson(ResourceMessageExceptions.UNKNOWN_ERROR));
    }
}