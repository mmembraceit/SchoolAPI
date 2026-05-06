using System.Net;

namespace StudentApi.Domain.Exceptions;

public sealed class BadRequestException(string errorCode, string message)
    : ApiException(HttpStatusCode.BadRequest, errorCode, message);
