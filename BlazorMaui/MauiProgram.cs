using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BlazorMaui.Services.Interfaces;
using BlazorMaui.Services;

namespace BlazorMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });


            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddScoped<HttpClient>();

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IBeatsService, BeatsService>();





#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
