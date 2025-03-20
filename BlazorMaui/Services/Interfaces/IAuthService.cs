namespace BlazorMaui.Services.Interfaces;

public interface IAuthService
{
    private const string TokenKey = "Token";
    public int UserId { get; set; }

    public Task<bool> IsUserAuthenticatedAsync();
    public Task UpdateUserId();


    public Task SaveTokenAsync(string token);


    public Task<string?> GetTokenAsync();
    public Task StoreUserId(int userId);

    public Task LogoutAsync();

}