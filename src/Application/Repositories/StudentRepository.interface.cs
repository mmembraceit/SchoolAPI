using StudentApi.Domain.Models.Students;

namespace StudentApi.Application.Repositories;

public interface IStudentRepository
{
    Task<StudentModel?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StudentModel>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string name, DateOnly dateOfBirth, Guid tenantId, CancellationToken cancellationToken);
    Task AddAsync(StudentModel student, CancellationToken cancellationToken);
    Task UpdateAsync(StudentModel student, CancellationToken cancellationToken);
    Task DeleteAsync(StudentModel student, CancellationToken cancellationToken);
}