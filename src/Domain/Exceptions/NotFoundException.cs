using System.Net;

namespace StudentApi.Domain.Exceptions;

public sealed class NotFoundException : ApiException
{
    public NotFoundException(string errorCode, string message)
        : base(HttpStatusCode.NotFound, errorCode, message) { }
}
