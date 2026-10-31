using System.Net;

namespace Globals.Exceptions;

public class BaseException : Exception
{
    public int StatusCode { get; }

    public BaseException(string message, int statusCode = (int)HttpStatusCode.BadRequest) 
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : BaseException
{
    public NotFoundException(string message = "Resource not found.") 
        : base(message, (int)HttpStatusCode.NotFound)
    {
    }
}

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message = "Access is forbidden.") 
        : base(message, (int)HttpStatusCode.Forbidden)
    {
    }
}
