using StudentApi.Application.DTOs.Students;
using StudentApi.Application.DTOs.Tenants;
using StudentApi.Domain.Models.Students;
using StudentApi.Domain.Models.Tenants;

namespace StudentApi.Application.DTOs;

public static class DtoMappingExtensions
{
    // Student mappings
    public static StudentResponse ToResponse(this StudentModel model) =>
        new(
            model.Entity.Id,
            model.Entity.Name,
            model.Entity.DateOfBirth,
            model.Entity.CreatedAt,
            model.Entity.UpdatedAt,
            model.Entity.IsDeleted);

    public static StudentModel ToModel(this StudentCreateRequest request) =>
        StudentModel.Create(request.Name, request.DateOfBirth);

    public static void ApplyTo(this StudentUpdateRequest request, StudentModel model)
    {
        model.Entity.Name = request.Name;
        model.Entity.DateOfBirth = request.DateOfBirth;
    }

    // Tenant mappings
    public static TenantResponse ToResponse(this TenantModel model) =>
        new(
            model.Entity.Id,
            model.Entity.Name,
            model.Entity.Description,
            model.Entity.CreatedAt,
            model.Entity.UpdatedAt,
            model.Entity.IsDeleted);

    public static TenantModel ToModel(this TenantCreateRequest request) =>
        TenantModel.Create(request.Name, request.Description);

    public static void ApplyTo(this TenantUpdateRequest request, TenantModel model)
    {
        model.Entity.Name = request.Name;
        model.Entity.Description = request.Description;
    }
}
