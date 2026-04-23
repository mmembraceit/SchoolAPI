using StudentApi.Application.Context;

namespace StudentApi.Api.Context;

public sealed class TenantContext : ITenantContext
{
    public Guid TenantId { get; internal set; }
}
