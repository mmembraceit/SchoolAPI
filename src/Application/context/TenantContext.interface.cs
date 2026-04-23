namespace StudentApi.Application.Context;

public interface ITenantContext
{
    Guid TenantId { get; }
}
