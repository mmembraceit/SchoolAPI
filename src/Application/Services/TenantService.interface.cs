using StudentApi.Application.DTOs.Tenants;

namespace StudentApi.Application.Services;

public interface ITenantService
{
    Task<TenantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<TenantResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TenantResponse> CreateAsync(TenantCreateRequest request, CancellationToken cancellationToken);
    Task<TenantResponse?> UpdateAsync(Guid id, TenantUpdateRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
