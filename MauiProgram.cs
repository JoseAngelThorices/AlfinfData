using AlfinfData;
using AlfinfData.Services.odoo;
using AlfinfData.Services.BdLocal;
using AlfinfData.Settings;
using AlfinfData.ViewModels;
using AlfinfData.Views.Inicio;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net;

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

    {
        return new HttpClientHandler
        {
            // Guardamos cookies de sesión para poder usarla cada vez que hagamos peticiones a Odoo.
            CookieContainer = new CookieContainer(),
            UseCookies = true,

            // Esto sirve para poder usar conexiones https, pero evitando el validacion de certificación.
            ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    }
        );
        builder.Services.AddScoped<IEmpleadosService, EmpleadosService>();

        builder.Services.AddTransient<DescargasViewModel>();
        builder.Services.AddTransient<SeleccionViewModels>();
        builder.Services.AddTransient<DescargasPage>();

        // Base de datos local
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "BaseDeDatosLocal.db3");
        builder.Services.AddSingleton(sp => new DatabaseService(dbPath));

        // Repositorios de la base de datos local
        builder.Services.AddTransient<JornaleroRepository>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
