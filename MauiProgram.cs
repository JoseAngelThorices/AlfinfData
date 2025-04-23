using AlfinfData;
using AlfinfData.Services;
using AlfinfData.Settings;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
    



public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() 
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
       
        using var stream = FileSystem
            .OpenAppPackageFileAsync("appsettings.json")
            .GetAwaiter()
            .GetResult();

       
        builder.Configuration.AddJsonStream(stream);

      
        builder.Services.Configure<OdooConfiguracion>(
            builder.Configuration.GetSection("Odoo"));


        builder.Services.AddHttpClient<OdooService>((sp, client) =>
        {
            var cfg = sp.GetRequiredService<IOptions<OdooConfiguracion>>().Value;
            client.BaseAddress = new Uri(cfg.Url);
            client.Timeout = TimeSpan.FromSeconds(cfg.TimeoutSeconds);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler
    {
        // Acepta cualquier certificado SSL/TLS
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }
);



#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
