internal interface IJwtService
{

    public string GenerateJwtToken(string userId, string role);

    public Dictionary<string, string> ValidateToken(string token);
}