using System.Collections.ObjectModel;
using AlfinfData.Services.odoo;
using AlfinfData.Services.BdLocal;
using AlfinfData.Models.SQLITE;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlfinfData.Models.Odoo;
using System.Diagnostics;
using Plugin.NFC;
using System.Text;


namespace AlfinfData.ViewModels
{


    public partial class DescargasViewModel : ObservableObject
    {
        private readonly IEmpleadosService _empleadosService;     // servicio Odoo Empleado
        private readonly ICuadrillasService _cuadrillaService;    // servicio Odoo Cuadrilla
        private readonly JornaleroRepository _jornaleroRepo;      // repositorio SQLite
        private readonly CuadrillaRepository _cuadrillaRepo;
        public DescargasViewModel(IEmpleadosService empleadosService, ICuadrillasService cuadrillaService, JornaleroRepository jornaleroRepo, CuadrillaRepository cuadrillaRepo)
        {
            _empleadosService = empleadosService;
            _cuadrillaService = cuadrillaService;
            _jornaleroRepo = jornaleroRepo;
            _cuadrillaRepo = cuadrillaRepo;
            Empleados = new ObservableCollection<Empleado>();
            Cuadrillas = new ObservableCollection<CuadrillaOdoo>();
        }

        public ObservableCollection<Empleado> Empleados { get; }
        public ObservableCollection<CuadrillaOdoo> Cuadrillas { get; }
        public ObservableCollection<string> TagsLeidas { get; } = new();

        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        bool isAltaPopupVisible = false;
        [RelayCommand]
        private async Task NfcTarjeta()
        {
            // Comprueba que el usuario lo tenga activado
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
            CrossNFC.Current.OnMessageReceived += OnTagReceived;
            try
            {
                var valor = RangoValores();
                CrossNFC.Current.StartListening();
                IsAltaPopupVisible = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error NFC", ex.Message, "OK");
                return;
            }
        }


        void OnTagReceived(ITagInfo tagInfo)
        {
        // raw bytes
        var idBytes = tagInfo.Identifier;
        var idHex = BitConverter.ToString(idBytes);

        // serial como string
        var serial = tagInfo.SerialNumber;

        MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert(
                    "NFC leído",
                    $"ID bytes: {idHex}\nSerialNumber: {serial}",
                    "OK");
                });
        }

        async Task<(int start, int end)?> RangoValores()
        {
            // Pedir inicio
            var startStr = await Shell.Current.DisplayPromptAsync(
                "Rango NFC", "Valor de inicio:", "Aceptar", "Cancelar",
                placeholder: "0", keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(startStr))
                return null; // canceló

            // Pedir fin
            var endStr = await Shell.Current.DisplayPromptAsync(
                "Rango NFC", "Valor de fin:", "Aceptar", "Cancelar",
                placeholder: startStr, keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(endStr))
                return null; // canceló

            if (!int.TryParse(startStr, out var start) ||
                !int.TryParse(endStr, out var end) ||
                 start > end)
            {
                await Shell.Current.DisplayAlert(
                    "Error", "Rango inválido; inicio ≤ fin y números enteros.", "OK");
                return null;
            }

            return (start, end);
        }

        [RelayCommand]
        private async void CancelarAlta()
        {
            IsAltaPopupVisible = false;
            CrossNFC.Current.StopListening();
            CrossNFC.Current.OnMessageReceived -= OnTagReceived;

        }


        [RelayCommand]
        private async void Entrada()
        {
            await CargarEmpleadosAsync();
        }
        private async Task CargarEmpleadosAsync()
        {
            if (IsBusy)
                return;

            try
            {
                isBusy = true;

                var listaDesdeOdoo = await _empleadosService.GetAllAsync();
                var listaLocal = listaDesdeOdoo.Select(o => new Jornalero
                {
                    Nombre = o.Nombre,
                    IdCuadrilla = o.Id_Departamento,   
                    IdJornalero = o.Id,
                    Activo = o.Activo
                }).ToList(); // ahora es List<Jornalero>
                //Para ver los datos que se estan pasando por la terminal de salida
                //foreach (var j in listaLocal)
                //{
                //    Debug.WriteLine(
                //       $"[listaLocal] Nombre=\"{j.Nombre}\", IdCuadrilla={j.IdCuadrilla}, TarjetaNFC={j.TarjetaNFC}"
                //   );
                //}
                await _jornaleroRepo.UpsertJornalerosAsync(listaLocal);
                
                await Shell.Current.DisplayAlert("Success", "Se han bajado con exito los datos!", "OK");
            }
            catch (Exception ex)
            {
                // Mostrar alerta de error, log, etc.
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                isBusy = false;
            }
        }
        [RelayCommand]
        private async void Cuadrilla()
        {
            await CargarCuadrillaAsync();
        }
        private async Task CargarCuadrillaAsync()
        {
            if (IsBusy)
                return;

            try
            {
                isBusy = true;

                var listaDesdeOdoo = await _cuadrillaService.GetAllAsync();
                var listaLocal = listaDesdeOdoo.Select(o => new Cuadrilla
                {
                    Descripcion = o.Descripcion,
                    IdCuadrilla = o.IdCuadrilla
                }).ToList(); // ahora es List<Cuadrilla>
                //Para ver los datos que se estan pasando por la terminal de salida
                foreach (var j in listaLocal)
                {
                   Debug.WriteLine(
                        $"[listaLocal] IdOdoo={j.IdCuadrilla}, Nombre=\"{j.Descripcion}\""
                    );
                }
                await _cuadrillaRepo.UpsertCuadrillaAsync(listaLocal);

                await Shell.Current.DisplayAlert("Success", "Se han bajado con exito los datos!", "OK");
            }
            catch (Exception ex)
            {
                // Mostrar alerta de error, log, etc.
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}
