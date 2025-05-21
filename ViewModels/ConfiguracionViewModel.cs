using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlfinfData.ViewModels
{
    public partial class ConfiguracionViewModel : ObservableObject
    {
        private readonly IConfigService _config;

        public ConfiguracionViewModel(IConfigService config)
        {
            _config = config;

            // Disparamos la carga inicial de forma asíncrona
            _ = InitializeAsync();

            // Volver a cargar todo si cambia cualquier configuración
            _config.ConfigChanged += async (_, __) => await InitializeAsync();

        }

        [ObservableProperty] private string odooUrl;
        [ObservableProperty] private string port;
        [ObservableProperty] private string username;
        [ObservableProperty] private string password;
        [ObservableProperty] private string databaseName;

        private async Task InitializeAsync()
        {
            OdooUrl = _config.OdooUrl;
            Port = _config.OdooPort;

            var (user, pass, db) = await _config.GetCredentialsAsync();
            Username = user;
            Password = pass;
            DatabaseName = db;
        }

        [RelayCommand]
        public async Task EditarUrlAsync()
        {
            var resultado = await Shell.Current.DisplayPromptAsync(
                "Editar IP",
                "Introduce la nueva IP o URL del servidor:",
                accept: "Guardar",
                cancel: "Cancelar",
                placeholder: OdooUrl,
                maxLength: 100,
                keyboard: Keyboard.Url);

            if (string.IsNullOrWhiteSpace(resultado))
                return;

            // Validación básica: que empiece por http:// o https://
            if (!Uri.TryCreate(resultado, UriKind.Absolute, out _))
            {
                await Shell.Current.DisplayAlert(
                  "URL no válida",
                  "Debe incluir el esquema (http:// o https://).",
                  "OK");
                return;
            }

            // Guardamos en Preferences (dispara ConfigChanged)
            _config.OdooUrl = resultado;
            await DisplayGuardadoAsync("Url");
        }

        [RelayCommand]
        public async Task EditarPuertoAsync()
        {
            var resultado = await Shell.Current.DisplayPromptAsync(
                "Editar Puerto",
                "Introduce el nuevo puerto del servidor:",
                accept: "Guardar",
                cancel: "Cancelar",
                placeholder: Port,
                maxLength: 5,
                keyboard: Keyboard.Numeric);

            if (!int.TryParse(resultado, out var p) || p < 1 || p > 65535)
            {
                await Shell.Current.DisplayAlert(
                  "Puerto no válido",
                  "Debe ser un número entre 1 y 65535.",
                  "OK");
                return;
            }

            _config.OdooPort = resultado;
            await DisplayGuardadoAsync("Puerto");
        }
        [RelayCommand]
        public async Task EditarUsuarioOdooAsync()
        {
            var resultado = await Shell.Current.DisplayPromptAsync(
                "Editar Usuario",
                "Introduce el nuevo usuario de Odoo:",
                accept: "Guardar",
                cancel: "Cancelar",
                placeholder: Username,
                maxLength: 50,
                keyboard: Keyboard.Text);

            if (!string.IsNullOrWhiteSpace(resultado))
            {
                // Conservamos pass y db actuales al guardar
                await _config.SetCredentialsAsync(resultado, Password, DatabaseName);
                await InitializeAsync();
                await DisplayGuardadoAsync("Usuario");
            }
        }

        [RelayCommand]
        public async Task EditarContrasenaOdooAsync()
        {
            var resultado = await Shell.Current.DisplayPromptAsync(
                title: "Editar Contraseña",
                message: "Introduce la nueva contraseña de Odoo:",
                accept: "Guardar",
                cancel: "Cancelar",
                placeholder: Password,
                maxLength: 100,
                keyboard: Keyboard.Text,
                initialValue: Password
                );

            if (!string.IsNullOrWhiteSpace(resultado))
            {
                await _config.SetCredentialsAsync(Username, resultado, DatabaseName);
                await InitializeAsync();
                await DisplayGuardadoAsync("Contraseña");
            }
        }

        [RelayCommand]
        public async Task EditarNombreBaseDatosAsync()
        {
            var resultado = await Shell.Current.DisplayPromptAsync(
                "Editar Base de Datos",
                "Introduce el nombre de la base de datos de Odoo:",
                accept: "Guardar",
                cancel: "Cancelar",
                placeholder: DatabaseName,
                maxLength: 100,
                keyboard: Keyboard.Text);

            if (!string.IsNullOrWhiteSpace(resultado))
            {
                await _config.SetCredentialsAsync(Username, Password, resultado);
                await InitializeAsync();
                await DisplayGuardadoAsync("Base de datos");
            }
        }

        // Método auxiliar para alertar guardado
        private static Task DisplayGuardadoAsync(string campo) =>
            Shell.Current.DisplayAlert(
                "Configuración",
                $"{campo} guardado correctamente.",
                "OK");
    }

}
