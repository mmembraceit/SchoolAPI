using System.Net;

namespace StudentApi.Domain.Exceptions;

public sealed class ConflictException : ApiException
{
    public ConflictException(string errorCode, string message)
        : base(HttpStatusCode.Conflict, errorCode, message) { }
}
