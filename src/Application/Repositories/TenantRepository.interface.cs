using StudentApi.Domain.Models.Tenants;

namespace StudentApi.Application.Repositories;

public interface ITenantRepository
{
    Task<TenantModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<TenantModel>> GetAllAsync(CancellationToken cancellationToke);
    Task AddAsync(TenantModel tenant, CancellationToken cancellationToken);
    Task UpdateAsync(TenantModel tenant, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}