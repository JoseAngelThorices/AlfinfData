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
        private readonly List<int> _rangoValores = new List<int>();
        public ObservableCollection<Empleado> Empleados { get; }
        public ObservableCollection<CuadrillaOdoo> Cuadrillas { get; }
        public ObservableCollection<TarjetaNFC> TagsLeidas { get; } = new();
        private readonly List<string> _hexIds = new List<string>();
        private int _startValue;
        private int _endValue;
        public DescargasViewModel(IEmpleadosService empleadosService, ICuadrillasService cuadrillaService, JornaleroRepository jornaleroRepo, CuadrillaRepository cuadrillaRepo)
        {
            _empleadosService = empleadosService;
            _cuadrillaService = cuadrillaService;
            _jornaleroRepo = jornaleroRepo;
            _cuadrillaRepo = cuadrillaRepo;
            Empleados = new ObservableCollection<Empleado>();
            Cuadrillas = new ObservableCollection<CuadrillaOdoo>();
        }

       

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
                await SolicitarYRellenarRangoAsync();
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
            _hexIds.Add(serial);
            //MainThread.BeginInvokeOnMainThread(async () =>
            //    {
            //        await Shell.Current.DisplayAlert(
            //            "NFC leído",
            //            $"ID bytes: {idBytes}\nSerialNumber: {serial}",
            //            "OK");
            //    });
        }

        public async Task<(int start, int end)?> RangoValores()
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

        // 3) Método que llama a RangoValores y guarda el resultado
        public async Task SolicitarYRellenarRangoAsync()
        {
            var resultado = await RangoValores();
            if (resultado.HasValue)
            {
                _startValue = resultado.Value.start;
                _endValue = resultado.Value.end;

                // 1) Limpiamos la lista
                _rangoValores.Clear();

                // 2) La llenamos con cada valor entre start y end
                for (int i = _startValue; i <= _endValue; i++)
                {
                    _rangoValores.Add(i);
                }
            }
            else
            {
                // Cancelado o error…
            }
        }
        [RelayCommand]
        private async void CancelarAlta()
        {
            for (int i = 0;i < _hexIds.Count; i++)
            {
                    var tarjeta = new TarjetaNFC
                    {
                        IdTarjetaNFC = _rangoValores[i],
                        NumeroSerie = _hexIds[i],
                    };

                    // La añades a la colección
                    TagsLeidas.Add(tarjeta);
            }
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
                //foreach (var j in listaLocal)
                //{
                //   Debug.WriteLine(
                //        $"[listaLocal] IdOdoo={j.IdOdoo}, Nombre=\"{j.Descripcion}\""
                //    );
                //}
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
