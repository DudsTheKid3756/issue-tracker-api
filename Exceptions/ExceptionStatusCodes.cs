using System.Net;

namespace IssueTracker.Exceptions;

public static class ExceptionStatusCodes
{
    private static readonly Dictionary<Type, HttpStatusCode> StatusCodes = new()
    {
        {typeof(InvalidException), HttpStatusCode.BadRequest},
        {typeof(NotFoundException), HttpStatusCode.NotFound}
    };

    public static HttpStatusCode GetExceptionStatusCode(Exception exception)
    {
        var exceptionFound = StatusCodes.TryGetValue(exception.GetType(), out var statusCode);
        return exceptionFound ? statusCode : HttpStatusCode.InternalServerError;
    }
}