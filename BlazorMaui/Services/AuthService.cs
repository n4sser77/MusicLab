using BlazorMaui.Services.Interfaces;
using Microsoft.Maui.Storage;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMaui.Services;

public class AuthService : IAuthService
{
    private const string TokenKey = "Token";
    public int UserId { get; set; }

    public async Task UpdateUserId()
    {
        using var client = new HttpClient();
        var token = await GetTokenAsync();
        client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

        var rawRes = await client.GetAsync("http://192.168.1.174:5106/api/auth/profile");
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResponse = await rawRes.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(jsonResponse, options);

        await StoreUserId(user.Id);
    }

    public async Task<bool> IsUserAuthenticatedAsync()
    {
        var token = await SecureStorage.GetAsync(TokenKey);
        bool isAuthenticated = !string.IsNullOrEmpty(token);

        return isAuthenticated;
    }

    public async Task SaveTokenAsync(string token)
    {
        await SecureStorage.SetAsync(TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync(TokenKey);
    }
    public async Task StoreUserId(int userId)
    {
        UserId = userId;
    }

    public async Task LogoutAsync()
    {
        SecureStorage.Remove(TokenKey);
        return;
    }




}
