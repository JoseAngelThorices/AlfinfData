using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using AlfinfData.Models.ConfiguracionApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace AlfinfData.ViewModels
{
    public partial class ConfiguracionViewModel : ObservableObject
    {
        private readonly IConfigService _config;

        public ConfiguracionViewModel(IConfigService config)
        {
            _config = config;
            // Carga inicial desde Preferences
            OdooUrl = _config.OdooUrl;
            Port = _config.OdooPort;

            // Si en algún otro punto se cambia la configuración
            _config.ConfigChanged += (_, __) =>
            {
                OdooUrl = _config.OdooUrl;
                Port = _config.OdooPort;
            };
        }

        [ObservableProperty]
        private string odooUrl;

        [ObservableProperty]
        private string port;

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
            await Shell.Current.DisplayAlert(
                "Configuración",
                "URL guardada correctamente.",
                "OK");
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
            await Shell.Current.DisplayAlert(
                "Configuración",
                "Puerto guardado correctamente.",
                "OK");
        }
    }

}
