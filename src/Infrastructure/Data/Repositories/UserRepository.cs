using Microsoft.EntityFrameworkCore;
using StudentApi.Application.Repositories;
using StudentApi.Domain.Models.Users;
using StudentApi.Infrastructure.Data;

namespace StudentApi.Infrastructure.Repositories;

public class UserRepository(StudentApiDbContext dbContext) : IUserRepository
{
    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
