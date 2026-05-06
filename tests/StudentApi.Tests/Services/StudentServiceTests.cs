using NSubstitute;
using StudentApi.Application.Context;
using StudentApi.Application.DTOs.Students;
using StudentApi.Application.Hubs;
using StudentApi.Application.Messaging;
using StudentApi.Application.Messaging.Events;
using StudentApi.Application.Repositories;
using StudentApi.Application.Services;
using StudentApi.Domain.Exceptions;
using StudentApi.Domain.Models.Students;

namespace StudentApi.Tests.Services;

public class StudentServiceTests
{
    private readonly IStudentRepository _repository = Substitute.For<IStudentRepository>();
    private readonly ITenantContext _tenantContext = Substitute.For<ITenantContext>();
    private readonly IMessagePublisher _publisher = Substitute.For<IMessagePublisher>();
    private readonly IStudentHubContext _hub = Substitute.For<IStudentHubContext>();
    private readonly StudentService _sut;

    private static readonly Guid TenantId = Guid.NewGuid();

    public StudentServiceTests()
    {
        _tenantContext.TenantId.Returns(TenantId);
        _sut = new StudentService(_repository, _tenantContext, _publisher, _hub);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenStudentExists_ReturnsResponse()
    {
        var model = BuildStudent("Alice");
        _repository.GetByIdAsync(model.Entity.Id, TenantId, default)
            .Returns(model);

        var result = await _sut.GetByIdAsync(model.Entity.Id, default);

        Assert.Equal(model.Entity.Id, result.Id);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStudentMissing_ThrowsNotFoundException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), TenantId, default)
            .Returns((StudentModel?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.GetByIdAsync(Guid.NewGuid(), default));
    }

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllTenantStudents()
    {
        var models = new List<StudentModel> { BuildStudent("Alice"), BuildStudent("Bob") };
        _repository.GetAllAsync(TenantId, default).Returns(models);

        var result = await _sut.GetAllAsync(default);

        Assert.Equal(2, result.Count);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidRequest_AddsStudentAndPublishesEvent()
    {
        var request = new StudentCreateRequest("Charlie", new DateOnly(2000, 1, 1));
        _repository.ExistsAsync(request.Name, request.DateOfBirth, TenantId, default)
            .Returns(false);

        var result = await _sut.CreateAsync(request, default);

        Assert.Equal("Charlie", result.Name);
        await _repository.Received(1).AddAsync(Arg.Any<StudentModel>(), default);
        await _publisher.Received(1).PublishAsync(Arg.Any<StudentCreatedEvent>(), default);
        await _hub.Received(1).NotifyStudentCreatedAsync(Arg.Any<StudentCreatedEvent>(), default);
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateExists_ThrowsConflictException()
    {
        var request = new StudentCreateRequest("Alice", new DateOnly(2000, 1, 1));
        _repository.ExistsAsync(request.Name, request.DateOfBirth, TenantId, default)
            .Returns(true);

        await Assert.ThrowsAsync<ConflictException>(
            () => _sut.CreateAsync(request, default));

        await _repository.DidNotReceive().AddAsync(Arg.Any<StudentModel>(), default);
    }

    // ── UpdateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenStudentExists_UpdatesAndPublishesEvent()
    {
        var model = BuildStudent("Alice");
        _repository.GetByIdAsync(model.Entity.Id, TenantId, default).Returns(model);

        var request = new StudentUpdateRequest("Alice Updated", model.Entity.DateOfBirth);
        var result = await _sut.UpdateAsync(model.Entity.Id, request, default);

        await _repository.Received(1).UpdateAsync(model, default);
        await _publisher.Received(1).PublishAsync(Arg.Any<StudentUpdatedEvent>(), default);
        await _hub.Received(1).NotifyStudentUpdatedAsync(Arg.Any<StudentUpdatedEvent>(), default);
    }

    [Fact]
    public async Task UpdateAsync_WhenStudentMissing_ThrowsNotFoundException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), TenantId, default)
            .Returns((StudentModel?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.UpdateAsync(Guid.NewGuid(), new StudentUpdateRequest("X", DateOnly.MinValue), default));
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenStudentExists_DeletesAndPublishesEvent()
    {
        var model = BuildStudent("Alice");
        _repository.GetByIdAsync(model.Entity.Id, TenantId, default).Returns(model);

        await _sut.DeleteAsync(model.Entity.Id, default);

        await _repository.Received(1).DeleteAsync(model, default);
        await _publisher.Received(1).PublishAsync(Arg.Any<StudentDeletedEvent>(), default);
        await _hub.Received(1).NotifyStudentDeletedAsync(Arg.Any<StudentDeletedEvent>(), default);
    }

    [Fact]
    public async Task DeleteAsync_WhenStudentMissing_ThrowsNotFoundException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), TenantId, default)
            .Returns((StudentModel?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.DeleteAsync(Guid.NewGuid(), default));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static StudentModel BuildStudent(string name)
        => StudentModel.Create(TenantId, name, new DateOnly(2000, 1, 1));
}
