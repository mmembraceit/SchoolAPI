using System.Net.Http.Headers;
using System.Net.Http.Json;
using StudentApi.Api.Endpoints;
using StudentApi.Api.Models;

namespace StudentApi.IntegrationTests.Helpers;

public static class AuthHelper
{
    public static async Task<HttpClient> CreateAuthenticatedClientAsync(
        ApiWebApplicationFactory factory,
        string email = "admin@oms-ltd.com",
        string password = "Admin@2026")
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<TokenResponse>>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body!.Data!.Token);

        return client;
    }
}
