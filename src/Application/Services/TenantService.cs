using StudentApi.Application.DTOs;
using StudentApi.Application.DTOs.Tenants;
using StudentApi.Application.Repositories;

namespace StudentApi.Application.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _repository;

    public TenantService(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<TenantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(id, cancellationToken);
        return tenant?.ToResponse();
    }

    public async Task<IEnumerable<TenantResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tenants = await _repository.GetAllAsync(cancellationToken);
        return tenants.Select(t => t.ToResponse());
    }

    public async Task<TenantResponse> CreateAsync(TenantCreateRequest request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        await _repository.AddAsync(model, cancellationToken);
        return model.ToResponse();
    }

    public async Task<TenantResponse?> UpdateAsync(Guid id, TenantUpdateRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(id, cancellationToken);
        if (tenant is null) return null;

        request.ApplyTo(tenant);
        await _repository.UpdateAsync(tenant, cancellationToken);
        return tenant.ToResponse();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(id, cancellationToken);
        if (tenant is null) return false;

        await _repository.DeleteAsync(id, cancellationToken);
        return true;
    }
}
