using System.Collections.ObjectModel;
using AlfinfData.Services.odoo;
using AlfinfData.Services.BdLocal;
using AlfinfData.Models.SQLITE;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlfinfData.Models.Odoo;
using Plugin.NFC;
using System.Diagnostics;

namespace AlfinfData.ViewModels
{
    public partial class DescargasViewModel : ObservableObject
    {
        // Dependencias y Repositorios
        private readonly IEmpleadosService _empleadosService;
        private readonly ICuadrillasService _cuadrillaService;
        private readonly ITarjetaNFCServices _tarjetaNFCService;
        private readonly JornaleroRepository _jornaleroRepo;
        private readonly CuadrillaRepository _cuadrillaRepo;

        // Datos internos
        private readonly List<int> _rangoValores = new();
        private readonly List<string> _hexIds = new();
        private int _startValue;

        // Propiedades públicas (UI)
        public ObservableCollection<Empleado> Empleados { get; } = new();
        public ObservableCollection<CuadrillaOdoo> Cuadrillas { get; } = new();
        public ObservableCollection<TarjetaNFC> TagsLeidas { get; } = new();

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private bool isAltaPopupVisible;

        public DescargasViewModel(
            IEmpleadosService empleadosService,
            ICuadrillasService cuadrillaService,
            ITarjetaNFCServices tarjetaNFCService,
            JornaleroRepository jornaleroRepo,
            CuadrillaRepository cuadrillaRepo)
        {
            _empleadosService = empleadosService;
            _cuadrillaService = cuadrillaService;
            _tarjetaNFCService = tarjetaNFCService;
            _jornaleroRepo = jornaleroRepo;
            _cuadrillaRepo = cuadrillaRepo;
        }

        // === LECTURA NFC ===

        [RelayCommand]
        private async Task NfcTarjeta()
        {
            if (!CrossNFC.Current.IsAvailable)
            {
                await Shell.Current.DisplayAlert("NFC", "Este dispositivo no tiene NFC.", "OK");
                return;
            }

            if (!CrossNFC.Current.IsEnabled)
            {
                await Shell.Current.DisplayAlert("NFC", "Activa NFC en los ajustes.", "OK");
                return;
            }

            try
            {
                CrossNFC.Current.OnMessageReceived += OnTagReceived;
                await SolicitarYRellenarRangoAsync();
                CrossNFC.Current.StartListening();
                IsAltaPopupVisible = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al activar NFC: {ex.Message}");
                await Shell.Current.DisplayAlert("Error NFC", "No se pudo iniciar la lectura NFC.", "OK");
            }
        }

        private void OnTagReceived(ITagInfo tagInfo)
        {
            var serial = tagInfo.SerialNumber;
            _hexIds.Add(serial);
        }

        public async Task<int> RangoValores()
        {
            var startStr = await Shell.Current.DisplayPromptAsync(
                "Rango NFC", "Valor de inicio:", "Aceptar", "Cancelar",
                placeholder: "0", keyboard: Keyboard.Numeric);

            if (string.IsNullOrWhiteSpace(startStr)) return 0;

            if (!int.TryParse(startStr, out var inicio))
            {
                await Shell.Current.DisplayAlert("Error", "El valor debe ser un número entero.", "OK");
                return 0;
            }

            return inicio;
        }

        public async Task SolicitarYRellenarRangoAsync()
        {
            _startValue = await RangoValores();
        }

        [RelayCommand]
        private async void CancelarAlta()
        {
            try
            {
                for (int i = 0; i < _hexIds.Count; i++)
                {
                    TagsLeidas.Add(new TarjetaNFC
                    {
                        IdTarjetaNFC = _startValue++,
                        NumeroSerie = _hexIds[i]
                    });
                }

                await _tarjetaNFCService.CreateTarjetasNFCAsync(TagsLeidas);

                TagsLeidas.Clear();
                _hexIds.Clear();
                IsAltaPopupVisible = false;
                CrossNFC.Current.StopListening();
                CrossNFC.Current.OnMessageReceived -= OnTagReceived;
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "No se ha podido conectar con el servidor.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // === ENTRADA DE DATOS DESDE ODOO ===

        [RelayCommand]
        private async void Entrada() => await CargarEmpleadosAsync();

        private async Task CargarEmpleadosAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var listaDesdeOdoo = await _empleadosService.GetAllAsync();

                var listaLocal = listaDesdeOdoo.Select(o => new Jornalero
                {
                    IdJornalero = o.Id,
                    Nombre = o.Nombre,
                    IdCuadrilla = o.Id_Departamento,
                    Activo = o.Activo,
                    TarjetaNFC = o.TarjetaNFC,
                }).ToList();

                await _jornaleroRepo.UpsertJornalerosAsync(listaLocal);
                await Shell.Current.DisplayAlert("Éxito", "Se han descargado los jornaleros.", "OK");
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "No se ha podido conectar con el servidor.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // === DESCARGA DE CUADRILLAS ===

        [RelayCommand]
        private async void Cuadrilla() => await CargarCuadrillaAsync();

        private async Task CargarCuadrillaAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var listaDesdeOdoo = await _cuadrillaService.GetAllAsync();

                var listaLocal = listaDesdeOdoo.Select(o => new Cuadrilla
                {
                    IdCuadrilla = o.IdCuadrilla,
                    Descripcion = o.Descripcion
                }).ToList();

                await _cuadrillaRepo.UpsertCuadrillaAsync(listaLocal);
                await Shell.Current.DisplayAlert("Éxito", "Se han descargado las cuadrillas.", "OK");
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "No se ha podido conectar con el servidor.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
