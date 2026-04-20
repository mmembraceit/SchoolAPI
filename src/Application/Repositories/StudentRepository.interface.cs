using StudentApi.Domain.Models.Students;

namespace StudentApi.Application.Repositories;

public interface IStudentRepository
{
    Task<StudentModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<StudentModel>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(StudentModel student, CancellationToken cancellationToken);
    Task UpdateAsync(StudentModel student, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}