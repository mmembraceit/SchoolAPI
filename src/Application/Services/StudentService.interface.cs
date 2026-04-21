using StudentApi.Application.DTOs.Students;

namespace StudentApi.Application.Services;

public interface IStudentService
{
    Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<StudentResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<StudentResponse> CreateAsync(StudentCreateRequest request, CancellationToken cancellationToken);
    Task<StudentResponse> UpdateAsync(Guid id, StudentUpdateRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
