using StudentApi.Api.Endpoints;
using StudentApi.Api.Middleware;
using StudentApi.Application;
using StudentApi.Infrastructure;

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

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapStudentEndpoints();
app.MapTenantEndpoints();

app.Run();
