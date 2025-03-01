using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleHttp.Services;
using Services.Interfaces;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;


public class Program
{
    public static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(op=> op.UseSqlServer(@"Server=.\SQLEXPRESS;Database=MyCloudServiceDB;Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=True"))
            .AddSingleton<IUserManager, UserManager>()
            .AddSingleton<IJwtService, JwtService>()
              .AddSingleton(new JsonSerializerOptions
              {
                  PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                  ReferenceHandler = ReferenceHandler.IgnoreCycles,
                  WriteIndented = true,
                  DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
              })
            .BuildServiceProvider();



        var server = new SimpleHttpServer(serviceProvider);
        await server.Start();  // Start the server
    }
}
