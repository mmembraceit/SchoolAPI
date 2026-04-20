using StudentApi.Domain.Models.Tenants;

namespace StudentApi.Application.Repositories;

public interface ITenantRepository
{
    Task<TenantModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TenantModel>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(TenantModel tenant, CancellationToken cancellationToken);
    Task UpdateAsync(TenantModel tenant, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}