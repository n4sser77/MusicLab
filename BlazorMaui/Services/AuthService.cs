using BlazorMaui.Services.Interfaces;
using Microsoft.Maui.Storage;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMaui.Services;

public class AuthService : IAuthService
{
    private const string TokenKey = "Token";
    public int UserId { get; set; }



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
