using System.Net;

namespace StudentApi.Domain.Exceptions;

public sealed class ForbiddenException : ApiException
{
    public ForbiddenException(string errorCode, string message)
        : base(HttpStatusCode.Forbidden, errorCode, message) { }
}
