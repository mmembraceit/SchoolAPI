using System.Net;
using System.Net.Http.Json;
using StudentApi.Api.Models;
using StudentApi.Application.DTOs.Students;
using StudentApi.IntegrationTests.Helpers;

namespace StudentApi.IntegrationTests.Endpoints;

public class StudentEndpointsTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>
{
    // ── Auth guard ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetStudents_WithoutToken_Returns401()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/students");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── GET /api/students ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetStudents_Authenticated_ReturnsOmsStudentsOnly()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.GetAsync("/api/students");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<ApiResponse<IReadOnlyCollection<StudentResponse>>>();

        Assert.True(body!.Success);
        // Seed data has 2 students for OMS Ltd tenant.
        Assert.Equal(2, body.Data!.Count);
        Assert.Contains(body.Data, s => s.Name == "Alice Johnson");
        Assert.Contains(body.Data, s => s.Name == "Bob Smith");
    }

    // ── POST /api/students ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateStudent_ValidRequest_Returns201()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        var request = new StudentCreateRequest("New Student", new DateOnly(2005, 6, 15));
        var response = await client.PostAsJsonAsync("/api/students", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<StudentResponse>>();
        Assert.True(body!.Success);
        Assert.Equal("New Student", body.Data!.Name);
    }

    [Fact]
    public async Task CreateStudent_DuplicateNameAndDob_Returns409()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        var request = new StudentCreateRequest("Alice Johnson", new DateOnly(2008, 3, 15));
        var response = await client.PostAsJsonAsync("/api/students", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // ── GET /api/students/{id} ────────────────────────────────────────────────

    [Fact]
    public async Task GetStudentById_ExistingId_Returns200()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        // Alice Johnson is seeded with this fixed ID for OMS Ltd.
        var id = Guid.Parse("C0000000-0000-0000-0000-000000000001");
        var response = await client.GetAsync($"/api/students/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<StudentResponse>>();
        Assert.Equal("Alice Johnson", body!.Data!.Name);
    }

    [Fact]
    public async Task GetStudentById_NonExistingId_Returns404()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.GetAsync($"/api/students/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── PUT /api/students/{id} ────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStudent_ExistingId_Returns200WithUpdatedName()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);
        var id = Guid.Parse("C0000000-0000-0000-0000-000000000002"); // Bob Smith

        var request = new StudentUpdateRequest("Bob Updated", new DateOnly(2009, 7, 22));
        var response = await client.PutAsJsonAsync($"/api/students/{id}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<StudentResponse>>();
        Assert.Equal("Bob Updated", body!.Data!.Name);
    }

    // ── DELETE /api/students/{id} ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteStudent_ExistingId_Returns200ThenGetReturns404()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);
        var id = Guid.Parse("C0000000-0000-0000-0000-000000000001"); // Alice Johnson

        var deleteResponse = await client.DeleteAsync($"/api/students/{id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        // Soft-deleted student must not be returned anymore.
        var getResponse = await client.GetAsync($"/api/students/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
