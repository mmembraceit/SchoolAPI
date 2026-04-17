using Microsoft.EntityFrameworkCore;
using StudentApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("StudentApiDb")
    ?? throw new InvalidOperationException("Connection string 'StudentApiDb' was not found.");

builder.Services.AddDbContext<StudentApiDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
