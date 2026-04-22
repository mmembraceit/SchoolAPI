using Microsoft.EntityFrameworkCore;
using StudentApi.Api.Endpoints;
using StudentApi.Api.Middleware;
using StudentApi.Application;
using StudentApi.Infrastructure;
using StudentApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudentApiDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (!app.Environment.IsEnvironment("Docker"))
    app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapStudentEndpoints();
app.MapTenantEndpoints();

app.Run();
