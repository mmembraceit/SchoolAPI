namespace   StudentApi.Domain.ErrorCodes;

public static partial class StudentApiErrorCodes
{
    public static class Student
    {
        public const string NotFound = "error.student.not_found";
        public const string NameAlreadyExists = "error.student.name_already_exists";
    }
}