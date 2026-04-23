using Microsoft.EntityFrameworkCore;
using StudentApi.Application.Repositories;
using StudentApi.Domain.Models.Students;
using StudentApi.Infrastructure.Data;

namespace StudentApi.Infrastructure.Repositories;

public class StudentRepository(StudentApiDbContext dbContext) : IStudentRepository
{
    public async Task<StudentModel?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId, cancellationToken);

        return entity is null ? null : StudentModel.FromEntity(entity);
    }

    public async Task<IReadOnlyList<StudentModel>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var entities = await dbContext.Students
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        return entities.Select(StudentModel.FromEntity).ToList();
    }

    public async Task AddAsync(StudentModel student, CancellationToken cancellationToken)
    {
        await dbContext.Students.AddAsync(student.Entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(StudentModel student, CancellationToken cancellationToken)
    {
        dbContext.Students.Update(student.Entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Students.FirstOrDefaultAsync(student => student.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.MarkAsDeleted();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}