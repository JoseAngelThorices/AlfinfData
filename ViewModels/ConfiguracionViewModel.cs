using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using AlfinfData.Models.ConfiguracionApp;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlfinfData.ViewModels
{
    public partial class ConfiguracionViewModel : ObservableObject
    {
        //[ObservableProperty]
        //private string odooUrl = string.Empty;
        //public ConfiguracionViewModel()
        //{
        //    _ = InitializeAsync();
        //}
        //private class RootConfigDto
        //{
        //    public ConfigOdoo Odoo { get; set; } = new();
        //}

        //// Método principal de carga
        //private async Task InitializeAsync()
        //{
        //    try
        //    {
        //        // Abre tu archivo JSON embebido en la app
        //        using var stream = await FileSystem.OpenAppPackageFileAsync("appsettings.json");
        //        using var reader = new StreamReader(stream);
        //        var json = await reader.ReadToEndAsync();

        //        // Deserializa a un DTO intermedio
        //        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //        var dto = JsonSerializer.Deserialize<RootConfigDto>(json, options)
        //                  ?? throw new InvalidOperationException("JSON mal formado");

        //        // Asigna las propiedades al ViewModel
        //        OdooUrl = dto.Odoo.Url;
        //        OdooDatabase = dto.Odoo.Database;
        //        OdooUsername = dto.Odoo.Username;
        //        OdooPassword = dto.Odoo.Password;
        //        OdooTimeoutSeconds = dto.Odoo.TimeoutSeconds;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Maneja errores (log, user alert, etc.)
        //        System.Diagnostics.Debug.WriteLine($"Error cargando configuración: {ex}");
        //    }
        //}
        //public string OdooUrl
        //{
        //    get => _odooUrl;
        //    private set => SetProperty(ref _odooUrl, value);
        //}
        //private string _odooUrl;

        //public string OdooDatabase
        //{
        //    get => _odooDatabase;
        //    private set => SetProperty(ref _odooDatabase, value);
        //}
        //private string _odooDatabase;

        //public string OdooUsername
        //{
        //    get => _odooUsername;
        //    private set => SetProperty(ref _odooUsername, value);
        //}
        //private string _odooUsername;

        //public string OdooPassword
        //{
        //    get => _odooPassword;
        //    private set => SetProperty(ref _odooPassword, value);
        //}
        //private string _odooPassword;

        //public int OdooTimeoutSeconds
        //{
        //    get => _odooTimeoutSeconds;
        //    private set => SetProperty(ref _odooTimeoutSeconds, value);
        //}
        //private int _odooTimeoutSeconds;


    }
}
