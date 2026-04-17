namespace StudentApi.Application.DTOs.Students;

public sealed record StudentResponse(
    Guid Id,
    string Name,
    DateOnly DateOfBirth,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    bool IsDeleted);

public sealed record StudentCreateRequest(string Name, DateOnly DateOfBirth);

public sealed record StudentUpdateRequest(string Name, DateOnly DateOfBirth);
