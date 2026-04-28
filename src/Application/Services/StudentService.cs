using StudentApi.Application.Context;
using StudentApi.Application.DTOs;
using StudentApi.Application.DTOs.Students;
using StudentApi.Application.Messaging;
using StudentApi.Application.Messaging.Events;
using StudentApi.Application.Repositories;
using StudentApi.Domain.ErrorCodes;
using StudentApi.Domain.Exceptions;

namespace StudentApi.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IMessagePublisher _publisher;

    public StudentService(
        IStudentRepository repository,
        ITenantContext tenantContext,
        IMessagePublisher publisher)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _publisher = publisher;
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
        var model = request.ToModel(_tenantContext.TenantId);
        await _repository.AddAsync(model, cancellationToken);

        await _publisher.PublishAsync(new StudentCreatedEvent(
            StudentId:   model.Entity.Id,
            TenantId:    model.Entity.TenantId,
            Name:        model.Entity.Name,
            DateOfBirth: model.Entity.DateOfBirth,
            CreatedAt:   model.Entity.CreatedAt), cancellationToken);

        return model.ToResponse();
    }

    public async Task<StudentResponse> UpdateAsync(Guid id, StudentUpdateRequest request, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, _tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");

        request.ApplyTo(student);
        await _repository.UpdateAsync(student, cancellationToken);

        await _publisher.PublishAsync(new StudentUpdatedEvent(
            StudentId:   student.Entity.Id,
            TenantId:    student.Entity.TenantId,
            Name:        student.Entity.Name,
            DateOfBirth: student.Entity.DateOfBirth,
            UpdatedAt:   student.Entity.UpdatedAt), cancellationToken);

        return student.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, _tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");

        await _repository.DeleteAsync(student.Entity.Id, cancellationToken);

        await _publisher.PublishAsync(new StudentDeletedEvent(
            StudentId: student.Entity.Id,
            TenantId:  student.Entity.TenantId,
            DeletedAt: DateTimeOffset.UtcNow), cancellationToken);
    }
}
