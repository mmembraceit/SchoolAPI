namespace   StudentApi.Domain.ErrorCodes;

public static partial class StudentApiErrorCodes
{
    public static class Tenant
    {
        public const string NotFound = "error.tenant.not_found";
        public const string NameAlreadyExists = "error.tenant.name_already_exists";
    }
}