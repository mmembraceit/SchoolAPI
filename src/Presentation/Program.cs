using Microsoft.EntityFrameworkCore;
using StudentApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddDbContext<StudentApiDbContext>(options =>
    options.UseInMemoryDatabase("StudentApiDb"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
