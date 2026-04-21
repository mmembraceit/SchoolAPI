using System.Net;

namespace StudentApi.Domain.Exceptions;

public sealed class BusinessLogicException : ApiException
{
    public BusinessLogicException(string errorCode, string message)
        : base(HttpStatusCode.Conflict, errorCode, message) { }
}
