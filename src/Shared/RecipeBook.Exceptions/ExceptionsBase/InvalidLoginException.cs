using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.Exceptions.ExceptionsBase;

public class InvalidLoginException : RecipeBookException
{
    public InvalidLoginException() : base(ResourceMessageExceptions.EMAIL_OR_PASSWORD_INVALID)
    {
    }
}