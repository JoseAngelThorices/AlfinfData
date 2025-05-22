using AlfinfData;
using AlfinfData.Services.odoo;
using AlfinfData.Services.BdLocal;
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

        // Configura la aplicación MAUI
        
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // Activa el CommunityToolkit
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        // 1) Registrar ConfigService
        builder.Services.AddSingleton<IConfigService, ConfigService>();

        // 2) Named client “Odoo” con handler personalizado
        builder.Services
          .AddHttpClient("Odoo", (sp, client) =>
          {
              var cfg = sp.GetRequiredService<IConfigService>();
              client.BaseAddress = new Uri($"{cfg.OdooUrl}:{cfg.OdooPort}/");
              client.Timeout = TimeSpan.FromSeconds(30);
              client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
          })
          .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
          {
              CookieContainer = new CookieContainer(),
              UseCookies = true,
              ServerCertificateCustomValidationCallback =
                  HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
          });

        // 3) Registrar tu servicio, que pedirá siempre el cliente “Odoo”
        builder.Services.AddTransient<OdooService>();


        // Servicios de negocio (inyección de dependencias)
        builder.Services.AddScoped<IEmpleadosService, EmpleadosService>();
        builder.Services.AddScoped<ICuadrillasService, CuadrillaService>();
        builder.Services.AddScoped<ITarjetaNFCServices, TarjetaNFCServices>();
        // ViewModels y páginas para navegación e inyección
        builder.Services.AddTransient<DescargasViewModel>();
        builder.Services.AddTransient<EntradaViewModel>();
        builder.Services.AddTransient<SeleccionViewModels>();
        builder.Services.AddTransient<SalidasViewModel>();
        builder.Services.AddTransient<ProduccionViewModel>();
        builder.Services.AddTransient<FinViewModel>();
        builder.Services.AddTransient<HorasViewModel>();
        builder.Services.AddTransient<ConfiguracionViewModel>();


        builder.Services.AddTransient<DescargasPage>();
        builder.Services.AddTransient<InicioViewModel>();
        builder.Services.AddTransient<ProduccionViewModel>();


        // Configuración de base de datos local SQLite
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "BaseDeDatosLocal.db3");
        builder.Services.AddSingleton(sp => new DatabaseService(dbPath));

        // Repositorios para acceder a datos desde la base local
        builder.Services.AddTransient<JornaleroRepository>();
        builder.Services.AddTransient<CuadrillaRepository>();
        builder.Services.AddTransient<FichajeRepository>();
        builder.Services.AddTransient<ProduccionRepository>();
        builder.Services.AddTransient<HorasRepository>();
        builder.Services.AddTransient<HistoricoRepository>();

#if DEBUG
        // Activar logging en modo depuración
        builder.Logging.AddDebug();
#endif

        // Devolver la app ya configurada
        return builder.Build();
    }
}