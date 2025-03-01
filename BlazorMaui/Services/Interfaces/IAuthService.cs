namespace BlazorMaui.Services.Interfaces;

public interface IAuthService
{
    private const string TokenKey = "Token";

    public Task<bool> IsUserAuthenticatedAsync();


    public Task SaveTokenAsync(string token);


    public Task<string?> GetTokenAsync();

    public Task LogoutAsync();

}