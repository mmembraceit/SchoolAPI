using StudentApi.Application.Context;
using StudentApi.Application.DTOs;
using StudentApi.Application.DTOs.Students;
using StudentApi.Application.Repositories;
using StudentApi.Domain.ErrorCodes;
using StudentApi.Domain.Exceptions;

namespace StudentApi.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly ITenantContext _tenantContext;

    public StudentService(IStudentRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, _tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");
        return student.ToResponse();
    }

    public async Task<IReadOnlyCollection<StudentResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var students = await _repository.GetAllAsync(_tenantContext.TenantId, cancellationToken);
        return students.Select(s => s.ToResponse()).ToList();
    }

    public async Task<StudentResponse> CreateAsync(StudentCreateRequest request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsWithNameAsync(_tenantContext.TenantId, request.Name, cancellationToken))
            throw new ConflictException(
                StudentApiErrorCodes.Student.NameAlreadyExists,
                $"A student with name '{request.Name}' already exists.");

        var model = request.ToModel(_tenantContext.TenantId);
        await _repository.AddAsync(model, cancellationToken);
        return model.ToResponse();
    }

    public async Task<StudentResponse> UpdateAsync(Guid id, StudentUpdateRequest request, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, _tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");

        if (!string.Equals(student.Entity.Name, request.Name, StringComparison.Ordinal)
            && await _repository.ExistsWithNameAsync(_tenantContext.TenantId, request.Name, cancellationToken))
        {
            throw new ConflictException(
                StudentApiErrorCodes.Student.NameAlreadyExists,
                $"A student with name '{request.Name}' already exists.");
        }

        request.ApplyTo(student);
        await _repository.UpdateAsync(student, cancellationToken);
        return student.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, _tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");

        await _repository.DeleteAsync(student.Entity.Id, cancellationToken);
    }
}
