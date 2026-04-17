namespace StudentApi.Domain.ErrorCodes;

public static partial class StudentApiErrorCodes
{
    public static class General
    {

    public const string Unknown = "error.general.unknown";
    public const string AlreadyExists = "error.general.already_exists";
    public const string InvalidRequest = "error.general.invalid_request";
    public const string NotFound = "error.general.not_found";
    }
}