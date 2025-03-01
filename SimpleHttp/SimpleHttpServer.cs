using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using SimpleHttp.Services;
using static SimpleHttpServerHelpers;
using System.Security.Cryptography;
using Services.Interfaces;

public class SimpleHttpServer
{
    private readonly HttpListener _listener;
    private readonly IServiceProvider _serviceProvider;
    private readonly string url = "http://localhost:5080/";
    public SimpleHttpServer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"{url}");

    }

    public async Task Start()
    {
        _listener.Start();

        Console.WriteLine("Server listening on " + url);

        while (true)
        {
            var context = _listener.GetContext(); // Wait for incoming requests
            await ProcessRequest(context);
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        var response = context.Response;

        // Enable CORS
        response.Headers.Add("Access-Control-Allow-Origin", "*"); // Allow all origins
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

        if (context.Request.HttpMethod == "OPTIONS")
        {
            // Handle preflight request
            response.StatusCode = 204; // No Content
            await response.OutputStream.FlushAsync();
            response.Close();
            return;
        }

        var path = context.Request.Url.AbsolutePath.ToLower();
        string responseString = string.Empty;

        switch (path)
        {
            case "/login":
                responseString = await Login(context);
                break;
            case "/signup":
                responseString = await SignUp(context);
                break;
            case "/userprofile":
                responseString = await GetUserProfile(context);
                break;
            case "/adminpanel":
                responseString = await AdminPanel(context);
                break;
            default:
                responseString = "404 Not Found, see yaa!!";
                context.Response.StatusCode = 404;
                break;
        }

        try
        {

            var buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }
        catch (Exception)
        {

            throw;
        }
    }






    // Endpoint: User login
    private async Task<string> Login(HttpListenerContext context)
    {
        using StreamReader requestReader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
        string requstBody = await requestReader.ReadToEndAsync();
        var userManager = _serviceProvider.GetRequiredService<IUserManager>();
        var jwtService = _serviceProvider.GetRequiredService<IJwtService>();
        try
        {
            var userInfo = JsonSerializer.Deserialize<LogInModel>(requstBody);
            userInfo.Password = HashPassword(userInfo.Password);
            var user = await userManager.LogInUser(userInfo);
            if (user == null)
            {
                return JsonSerializer.Serialize(new { message = "Invalid credentials" });
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            var jwt = jwtService.GenerateJwtToken(user.Id.ToString(), user.Role);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return JsonSerializer.Serialize(new { token = jwt});
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return JsonSerializer.Serialize(new { status = context.Response.StatusDescription, message = "Error processing login request" });

        }

    }

    // Endpoint: User signup
    private async Task<string> SignUp(HttpListenerContext context)
    {
        using StreamReader streamReader = new StreamReader(context.Request.InputStream);
        string requestBody = await streamReader.ReadToEndAsync();
        var userManager = _serviceProvider.GetRequiredService<IUserManager>();
        var jwtService = _serviceProvider.GetRequiredService<IJwtService>();
        try
        {
            var userInfo = JsonSerializer.Deserialize<LogInModel>(requestBody);
            userInfo.Password = HashPassword(userInfo.Password);

            var user = await userManager.CreateUser(new User { Email = userInfo.Email, Password = userInfo.Password });

            if (user == null)

            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonSerializer.Serialize(new { message = "Failed to create user" });
            }
            var jwt = jwtService.GenerateJwtToken(user.Id.ToString(), "user");
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return jwt;
        }
        catch (Exception)
        {

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return JsonSerializer.Serialize(new { status = context.Response.StatusDescription, message = "Error processing  signup request" });
        }


    }

    // Endpoint: Get user profile
    private async Task<string> GetUserProfile(HttpListenerContext context)
    {
        try
        {
            var authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return JsonSerializer.Serialize(new { message = "Unauthorized: Missing or invalid token" });
            }

            string token = authHeader.Substring("Bearer ".Length);
            var jwtService = _serviceProvider.GetRequiredService<IJwtService>();

            var claims = jwtService.ValidateToken(token);
            if (claims == null)
            {
                return JsonSerializer.Serialize(new { message = "Unauthorized: Invalid token" });
            }

            string userId = claims[ClaimTypes.NameIdentifier];


            if (!int.TryParse(userId, out int userIdInt))
            {
                return JsonSerializer.Serialize(new { message = "Invalid user ID" });
            }
            var userManager = _serviceProvider.GetRequiredService<IUserManager>();
            var user = await userManager.GetUserById(userIdInt);

            if (user == null)
            {
                return JsonSerializer.Serialize(new { message = "User not found" });
            }

            return JsonSerializer.Serialize(user);
        }
        catch (Exception)
        {
            return JsonSerializer.Serialize(new { message = "Failed to get user details" });
        }
    }


    // Endpoint: Admin panel
    private async Task<string> AdminPanel(HttpListenerContext context)
    {
        var userManager = _serviceProvider.GetRequiredService<IUserManager>();

        throw new NotImplementedException();
    }
}
