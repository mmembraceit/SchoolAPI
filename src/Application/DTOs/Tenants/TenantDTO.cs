namespace StudentApi.Application.DTOs.Tenants;

public sealed record TenantResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    bool IsDeleted);

public sealed record TenantCreateRequest(string Name, string? Description);

public sealed record TenantUpdateRequest(string Name, string? Description);
