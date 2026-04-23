using StudentApi.Domain.Models.Users;

namespace StudentApi.Application.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
