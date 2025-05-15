using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using AlfinfData.Models.ConfiguracionApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlfinfData.ViewModels
{
    public partial class ConfiguracionViewModel : ObservableObject
    {
        // 1) Valor inicial que debería verse en la UI
        [ObservableProperty]
        private string _odooUrl = "Cargando...";

        // 2) Método que lee el JSON y asigna OdooUrl; genera LoadConfigCommand
        [RelayCommand]
        private async Task LoadConfigAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("archivos.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                var dto = JsonSerializer.Deserialize<RootConfigDto>(json,
                           new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                          ?? throw new InvalidOperationException("JSON mal formado");

                // Esto dispara el PropertyChanged y refresca el Label
                OdooUrl = dto.Odoo.Url;
            }
            catch (Exception ex)
            {
                // En caso de error mostramos el mensaje
                OdooUrl = $"Error: {ex.Message}";
            }
        }

        // 3) Método para abrir la URL; genera OpenUrlCommand
        [RelayCommand]
        private async Task OpenUrlAsync()
        {
            if (!string.IsNullOrWhiteSpace(OdooUrl))
                await Launcher.OpenAsync(OdooUrl);
        }

        // 4) En el constructor ejecutamos la carga automáticamente
        public ConfiguracionViewModel()
        {
            // El comando se llama LoadConfigCommand, no UrlCommand
            LoadConfigCommand.Execute(null);
        }

        // Clases para deserializar el JSON
        private class RootConfigDto { public ConfigOdoo Odoo { get; set; } = new(); }


    }
}
