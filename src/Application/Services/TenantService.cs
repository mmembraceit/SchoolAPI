using StudentApi.Application.DTOs;
using StudentApi.Application.DTOs.Tenants;
using StudentApi.Application.Repositories;
using StudentApi.Domain.ErrorCodes;
using StudentApi.Domain.Exceptions;

namespace StudentApi.Application.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _repository;

    public TenantService(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<TenantResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Tenant.NotFound, $"Tenant '{id}' was not found.");
        return tenant.ToResponse();
    }

    public async Task<IReadOnlyCollection<TenantResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tenants = await _repository.GetAllAsync(cancellationToken);
        return tenants.Select(t => t.ToResponse()).ToList();
    }

    public async Task<TenantResponse> CreateAsync(TenantCreateRequest request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsWithNameAsync(request.Name, cancellationToken))
            throw new ConflictException(StudentApiErrorCodes.Tenant.NameAlreadyExists, $"A tenant with name '{request.Name}' already exists.");

        var model = request.ToModel();
        await _repository.AddAsync(model, cancellationToken);
        return model.ToResponse();
    }

    public async Task<TenantResponse> UpdateAsync(Guid id, TenantUpdateRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Tenant.NotFound, $"Tenant '{id}' was not found.");

        request.ApplyTo(tenant);
        await _repository.UpdateAsync(tenant, cancellationToken);
        return tenant.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Tenant.NotFound, $"Tenant '{id}' was not found.");

        await _repository.DeleteAsync(tenant.Entity.Id, cancellationToken);
    }
}
