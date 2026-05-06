using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StudentApi.Application.Repositories;
using StudentApi.Domain.Models.Students;

namespace StudentApi.Infrastructure.Cache;

public sealed class CachedStudentRepository(
    IStudentRepository inner,
    IDistributedCache cache,
    ILogger<CachedStudentRepository> logger) : IStudentRepository
{
    private static readonly DistributedCacheEntryOptions DefaultExpiry = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    // ── Reads ─────────────────────────────────────────────────────────────────

    public async Task<StudentModel?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        var key = CacheKey.Student(id, tenantId);
        var cached = await TryGetAsync<StudentEntity>(key, cancellationToken);
        if (cached is not null)
            return StudentModel.FromEntity(cached);

        var model = await inner.GetByIdAsync(id, tenantId, cancellationToken);
        if (model is not null)
            await SetAsync(key, model.Entity, cancellationToken);

        return model;
    }

    public async Task<IReadOnlyList<StudentModel>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var key = CacheKey.StudentList(tenantId);
        var cached = await TryGetAsync<List<StudentEntity>>(key, cancellationToken);
        if (cached is not null)
            return cached.Select(StudentModel.FromEntity).ToList();

        var models = await inner.GetAllAsync(tenantId, cancellationToken);
        await SetAsync(key, models.Select(m => m.Entity).ToList(), cancellationToken);

        return models;
    }

    public Task<bool> ExistsAsync(string name, DateOnly dateOfBirth, Guid tenantId, CancellationToken cancellationToken)
        => inner.ExistsAsync(name, dateOfBirth, tenantId, cancellationToken);

    // ── Writes (invalidate after each operation) ──────────────────────────────

    public async Task AddAsync(StudentModel student, CancellationToken cancellationToken)
    {
        await inner.AddAsync(student, cancellationToken);
        await InvalidateListAsync(student.Entity.TenantId, cancellationToken);
    }

    public async Task UpdateAsync(StudentModel student, CancellationToken cancellationToken)
    {
        await inner.UpdateAsync(student, cancellationToken);
        await Task.WhenAll(
            InvalidateAsync(CacheKey.Student(student.Entity.Id, student.Entity.TenantId), cancellationToken),
            InvalidateListAsync(student.Entity.TenantId, cancellationToken));
    }

    public async Task DeleteAsync(StudentModel student, CancellationToken cancellationToken)
    {
        await inner.DeleteAsync(student, cancellationToken);
        await Task.WhenAll(
            InvalidateAsync(CacheKey.Student(student.Entity.Id, student.Entity.TenantId), cancellationToken),
            InvalidateListAsync(student.Entity.TenantId, cancellationToken));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<T?> TryGetAsync<T>(string key, CancellationToken cancellationToken)
    {
        try
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes is null) return default;
            return JsonSerializer.Deserialize<T>(bytes);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache read failed for key '{Key}'. Falling through to database.", key);
            return default;
        }
    }

    private async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            await cache.SetAsync(key, bytes, DefaultExpiry, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache write failed for key '{Key}'. Continuing without cache.", key);
        }
    }

    private Task InvalidateAsync(string key, CancellationToken cancellationToken)
    {
        try { return cache.RemoveAsync(key, cancellationToken); }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache invalidation failed for key '{Key}'.", key);
            return Task.CompletedTask;
        }
    }

    private Task InvalidateListAsync(Guid tenantId, CancellationToken cancellationToken)
        => InvalidateAsync(CacheKey.StudentList(tenantId), cancellationToken);
}

internal static class CacheKey
{
    public static string Student(Guid id, Guid tenantId) => $"students:{id}:tenant:{tenantId}";
    public static string StudentList(Guid tenantId)      => $"students:tenant:{tenantId}";
}
