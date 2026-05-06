using StudentApi.Application.Repositories;

namespace StudentApi.Infrastructure.Data;

public sealed class UnitOfWork(StudentApiDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
