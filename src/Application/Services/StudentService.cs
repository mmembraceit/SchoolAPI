using StudentApi.Application.Context;
using StudentApi.Application.DTOs;
using StudentApi.Application.DTOs.Students;
using StudentApi.Application.Hubs;
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
    private readonly IStudentHubContext _hub;

    public StudentService(
        IStudentRepository repository,
        ITenantContext tenantContext,
        IMessagePublisher publisher,
        IStudentHubContext hub)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _publisher = publisher;
        _hub = hub;
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
        var duplicate = await _repository.ExistsAsync(
            request.Name, request.DateOfBirth, _tenantContext.TenantId, cancellationToken);

        if (duplicate)
            throw new ConflictException(
                StudentApiErrorCodes.Student.AlreadyExists,
                $"A student named '{request.Name}' with the same date of birth already exists.");

        var model = request.ToModel(_tenantContext.TenantId);
        await _repository.AddAsync(model, cancellationToken);

        var createdEvent = new StudentCreatedEvent(
            StudentId:   model.Entity.Id,
            TenantId:    model.Entity.TenantId,
            Name:        model.Entity.Name,
            DateOfBirth: model.Entity.DateOfBirth,
            CreatedAt:   model.Entity.CreatedAt);

        await _publisher.PublishAsync(createdEvent, cancellationToken);
        await _hub.NotifyStudentCreatedAsync(createdEvent, cancellationToken);

        return model.ToResponse();
    }

    public async Task<StudentResponse> UpdateAsync(Guid id, StudentUpdateRequest request, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, _tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");

        request.ApplyTo(student);
        await _repository.UpdateAsync(student, cancellationToken);

        var updatedEvent = new StudentUpdatedEvent(
            StudentId:   student.Entity.Id,
            TenantId:    student.Entity.TenantId,
            Name:        student.Entity.Name,
            DateOfBirth: student.Entity.DateOfBirth,
            UpdatedAt:   student.Entity.UpdatedAt);

        await _publisher.PublishAsync(updatedEvent, cancellationToken);
        await _hub.NotifyStudentUpdatedAsync(updatedEvent, cancellationToken);

        return student.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await _repository.GetByIdAsync(id, _tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException(StudentApiErrorCodes.Student.NotFound, $"Student '{id}' was not found.");

        await _repository.DeleteAsync(student, cancellationToken);

        var deletedEvent = new StudentDeletedEvent(
            StudentId: student.Entity.Id,
            TenantId:  student.Entity.TenantId,
            DeletedAt: DateTimeOffset.UtcNow);

        await _publisher.PublishAsync(deletedEvent, cancellationToken);
        await _hub.NotifyStudentDeletedAsync(deletedEvent, cancellationToken);
    }
}
