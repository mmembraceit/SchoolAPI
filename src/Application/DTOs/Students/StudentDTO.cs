namespace StudentApi.Application.DTOs.Students;

public sealed record StudentResponse(
    Guid Id,
    Guid TenantId,
    string Name,
    DateOnly DateOfBirth,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    bool IsDeleted);

public sealed record StudentCreateRequest(Guid TenantId, string Name, DateOnly DateOfBirth);

public sealed record StudentUpdateRequest(string Name, DateOnly DateOfBirth);
