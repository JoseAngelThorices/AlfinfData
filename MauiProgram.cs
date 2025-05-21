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
    private const string ConfigFileName = "appsettings.json";
    public static MauiApp CreateMauiApp()
    {

        var destPath = Path.Combine(FileSystem.AppDataDirectory, ConfigFileName);
        if (!File.Exists(destPath))
        {
            // Lee el asset del bundle
            using var assetStream = FileSystem
                .OpenAppPackageFileAsync(ConfigFileName)
                .GetAwaiter()
                .GetResult();
            using var reader = new StreamReader(assetStream);
            var text = reader.ReadToEnd();
            // Escribe la copia en disco
            File.WriteAllText(destPath, text);
        }

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

        // Cargar archivo de configuración "appsettings.json"
        //using var stream = FileSystem
        //    .OpenAppPackageFileAsync("appsettings.json")
        //    .GetAwaiter()
        //    .GetResult();

        //// Añadir ese archivo a la configuración de la aplicación
        //builder.Configuration.AddJsonStream(stream);
        builder.Configuration
               .AddJsonFile(destPath, optional: false, reloadOnChange: true);
        // Enlazar sección "Odoo" del JSON a la clase OdooConfiguracion
        builder.Services.Configure<OdooConfiguracion>(
            builder.Configuration.GetSection("Odoo"));

        // Configurar un HttpClient especializado para OdooService
        builder.Services.AddHttpClient<OdooService>((sp, client) =>
        {
            var cfg = sp.GetRequiredService<IOptions<OdooConfiguracion>>().Value;
            client.BaseAddress = new Uri(cfg.Url); // URL del servidor Odoo
            client.Timeout = TimeSpan.FromSeconds(cfg.TimeoutSeconds); // Timeout configurable
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json")); // Cabecera de tipo JSON
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler
            {
                CookieContainer = new CookieContainer(), // Almacena cookies de sesión
                UseCookies = true,
                ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator // Permite certificados no válidos (solo en pruebas)
            };
        });


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