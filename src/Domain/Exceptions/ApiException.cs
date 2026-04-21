using System.Net;

namespace StudentApi.Domain.Exceptions;

public abstract class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }

    protected ApiException(HttpStatusCode statusCode, string errorCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
