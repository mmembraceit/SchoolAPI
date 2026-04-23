using StudentApi.Api.Context;
using StudentApi.Domain.ErrorCodes;
using StudentApi.Domain.Exceptions;

namespace StudentApi.Api.Middleware;

public class TenantContextMiddleware(RequestDelegate next)
{
    private const string TenantIdClaimType = "tenantId";

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst(TenantIdClaimType)?.Value;

            if (tenantIdClaim is null || !Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                throw new UnauthenticatedException(
                    StudentApiErrorCodes.General.InvalidRequest,
                    "TenantId claim is missing or invalid.");
            }

            tenantContext.TenantId = tenantId;
        }

        await next(context);
    }
}
