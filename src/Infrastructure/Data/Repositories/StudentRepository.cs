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

    public Task<bool> ExistsAsync(string name, DateOnly dateOfBirth, Guid tenantId, CancellationToken cancellationToken)
        => dbContext.Students.AnyAsync(
            s => s.Name == name && s.DateOfBirth == dateOfBirth && s.TenantId == tenantId,
            cancellationToken);

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

    public async Task DeleteAsync(StudentModel student, CancellationToken cancellationToken)
    {
        student.Entity.MarkAsDeleted();
        dbContext.Students.Update(student.Entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}