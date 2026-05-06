namespace StudentApi.Application.Services;

public interface ITokenService
{
    string GenerateToken(Guid tenantId, Guid userId, string role);
}