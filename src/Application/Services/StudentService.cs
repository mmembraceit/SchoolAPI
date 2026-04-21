using StudentApi.Application.DTOs;
using StudentApi.Application.DTOs.Students;
using StudentApi.Application.Repositories;
using StudentApi.Domain.ErrorCodes;
using StudentApi.Domain.Exceptions;

namespace StudentApi.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");
        return student.ToResponse();
    }

    public async Task<IEnumerable<StudentResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var students = await _repository.GetAllAsync(cancellationToken);
        return students.Select(s => s.ToResponse());
    }

    public async Task<StudentResponse> CreateAsync(StudentCreateRequest request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        await _repository.AddAsync(model, cancellationToken);
        return model.ToResponse();
    }

    public async Task<StudentResponse> UpdateAsync(Guid id, StudentUpdateRequest request, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");

        request.ApplyTo(student);
        await _repository.UpdateAsync(student, cancellationToken);
        return student.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");

        await _repository.DeleteAsync(student.Entity.Id, cancellationToken);
    }
}
