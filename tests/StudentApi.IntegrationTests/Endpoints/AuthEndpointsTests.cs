using System.Net;
using System.Net.Http.Json;
using StudentApi.Api.Endpoints;
using StudentApi.Api.Models;

namespace StudentApi.IntegrationTests.Endpoints;

public class AuthEndpointsTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_WithValidCredentials_Returns200AndToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("admin@oms-ltd.com", "Admin@2026"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<TokenResponse>>();
        Assert.True(body!.Success);
        Assert.NotEmpty(body.Data!.Token);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("admin@oms-ltd.com", "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("nobody@example.com", "Admin@2026"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
