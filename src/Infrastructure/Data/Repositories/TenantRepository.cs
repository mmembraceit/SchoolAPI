using Microsoft.EntityFrameworkCore;
using StudentApi.Application.Repositories;
using StudentApi.Domain.Models.Tenants;
using StudentApi.Infrastructure.Data;

namespace StudentApi.Infrastructure.Repositories;

public class TenantRepository(StudentApiDbContext dbContext) : ITenantRepository
{
    public async Task<TenantModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Tenants.SingleOrDefaultAsync(tenant => tenant.Id == id, cancellationToken);

        return entity is null ? null : TenantModel.FromEntity(entity);
    }

    public async Task<IEnumerable<TenantModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await dbContext.Tenants
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entities.Select(TenantModel.FromEntity);
    }

    public async Task AddAsync(TenantModel tenant, CancellationToken cancellationToken)
    {
        await dbContext.Tenants.AddAsync(tenant.Entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TenantModel tenant, CancellationToken cancellationToken)
    {
        dbContext.Tenants.Update(tenant.Entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Tenants.FirstOrDefaultAsync(tenant => tenant.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.MarkAsDeleted();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}