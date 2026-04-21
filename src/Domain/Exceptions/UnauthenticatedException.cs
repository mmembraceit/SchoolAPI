using System.Net;

namespace StudentApi.Domain.Exceptions;

public sealed class UnauthenticatedException : ApiException
{
    public UnauthenticatedException(string errorCode, string message)
        : base(HttpStatusCode.Unauthorized, errorCode, message) { }
}
