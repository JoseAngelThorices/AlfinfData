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
        // 1) Valor inicial que debería verse en la UI
        private const string ConfigFileName = "appsettings.json";
        [ObservableProperty]
        private string odooUrl = "Cargando...";
        [ObservableProperty]
        private string port = "Cargando...";

        // 2) Método que lee el JSON y asigna OdooUrl; genera LoadConfigCommand
        private string GetConfigPath()
        {
            // Ej: "/data/user/0/com.miapp/files/config.json"
            return Path.Combine(FileSystem.AppDataDirectory, ConfigFileName);
        }

        
        public async Task LoadConfigAsync()
        {

            try
            {
               
                var path = GetConfigPath();
                var json = File.ReadAllText(path);
                Debug.WriteLine("Contenido de config.json:");
                Debug.WriteLine(json);
                var dto = JsonSerializer.Deserialize<RootConfigDto>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                          ?? throw new InvalidOperationException("JSON mal formado");

                // Esto dispara el PropertyChanged y refresca el Label
                var uri = new Uri(dto.Odoo.Url);
                OdooUrl = uri.Host;
                Port = uri.Port.ToString();
            }
            catch (Exception ex)
            {
                // En caso de error mostramos el mensaje
                OdooUrl = $"Error: {ex.Message}";
            }
        }
        
        private async Task GuardarConfigJsonAsync()
        {
            try
            {
                var path = GetConfigPath();
                // 1) Leer la copia existente
                var json = File.ReadAllText(path);
                var appConfig = JsonSerializer.Deserialize<RootConfigDto>(json,
                                  new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                               ?? throw new InvalidOperationException("JSON mal formado");

                // 2) Sobreescribir sólo la URL nueva
                appConfig.Odoo.Url = $"http://{OdooUrl}:{Port}";

                // 3) Serializar con indentado (opcional) y escribir
                var options = new JsonSerializerOptions { WriteIndented = true };
                var newJson = JsonSerializer.Serialize(appConfig, options);
                Debug.WriteLine("Contenido de config.json:");
                Debug.WriteLine(newJson);
                File.WriteAllText(path, newJson);

                // Feedback al usuario, por ejemplo:
                await Shell.Current.DisplayAlert("Configuración", "Configuración guardada.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        [RelayCommand]
        public async Task EditarUrl()
        {
            string resultado = await Shell.Current.DisplayPromptAsync(
            title: "Editar IP",
            message: "Introduce la nueva IP del servidor:",
            accept: "Guardar",
            cancel: "Cancelar",
            placeholder: OdooUrl,
            initialValue: OdooUrl,
            maxLength: 50,
            keyboard: Keyboard.Chat);
            if (resultado != null)
            {
                OdooUrl = resultado;
                await GuardarConfigJsonAsync();
            }
        }
        [RelayCommand]
        public async Task EditarPuerto()
        {
            string resultado = await Shell.Current.DisplayPromptAsync(
            title: "Editar Puerto",
            message: "Introduce el nuevo puerto del servidor:",
            accept: "Guardar",
            cancel: "Cancelar",
            placeholder: Port,
            initialValue: Port,
            maxLength: 50,
            keyboard: Keyboard.Chat);
            if (resultado != null)
            {
                Port = resultado;
                await GuardarConfigJsonAsync();
            }
        }
        
        // Clases para deserializar el JSON
        private class RootConfigDto { public ConfigOdoo Odoo { get; set; } = new(); }


    }
}
