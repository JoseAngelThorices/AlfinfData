using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlfinfData.Services.BdLocal;
using AlfinfData.Models.SQLITE;
using Plugin.NFC;
using System.Diagnostics;
using System.Globalization;

namespace AlfinfData.ViewModels
{
    public partial class EntradaViewModel : ObservableObject
    {
        private readonly FichajeRepository _fichajeRepo;
        private readonly JornaleroRepository _jornaleroRepo;

        public ObservableCollection<JornaleroEntrada> JornalerosE { get; set; } = new();
        public ObservableCollection<Cuadrilla> Cuadrillas { get; set; } = new();

        [ObservableProperty]
        private string _horaTexto = "HORA";

        [ObservableProperty]
        private Cuadrilla? _cuadrillaSeleccionada;

        public EntradaViewModel(FichajeRepository fichajeRepo, JornaleroRepository jornaleroRepo)
        {
            _fichajeRepo = fichajeRepo;
            _jornaleroRepo = jornaleroRepo;
        }

        public async Task CargarCuadrillasAsync()
        {
            var cuadrillasBD = await _jornaleroRepo.GetCuadrillasConJornalerosAsync();
            Cuadrillas.Clear();

            // Insertamos "TODOS" como una cuadrilla falsa con ID 0
            Cuadrillas.Add(new Cuadrilla { IdCuadrilla = 0, Descripcion = "TODOS" });

            foreach (var c in cuadrillasBD)
                Cuadrillas.Add(c);

            CuadrillaSeleccionada = Cuadrillas.FirstOrDefault();
        }

        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla? value)
        {
            _ = CargarJornalerosSegunCuadrillaAsync();
        }
        
        public async Task CargarJornalerosSegunCuadrillaAsync()
        {
            if (CuadrillaSeleccionada == null)
                return;

            List<Jornalero> lista;

            if (CuadrillaSeleccionada.IdCuadrilla == 0) // TODOS
                lista = await _jornaleroRepo.GetJornalerosActivosAsync();
            else
                lista = await _jornaleroRepo.GetJornalerosActivosPorCuadrillaAsync(CuadrillaSeleccionada.IdCuadrilla);

            var fichaje = await _fichajeRepo.BuscarFichajeNuevoDiaDatos();

            JornalerosE.Clear();
            foreach (var j in lista)
            {
                JornalerosE.Add(new JornaleroEntrada
                {
                    IdJornalero = j.IdJornalero,
                    Nombre = j.Nombre,
                    HoraEficaz = fichaje.HoraEficaz
                });
            }
        }

        [RelayCommand]
        private async Task HoraButtonAsync()
        {
            string[] horas = new string[48];
            for (int i = 0; i < 24; i++)
            {
                horas[i * 2] = $"{i:D2}:00";
                horas[i * 2 + 1] = $"{i:D2}:30";
            }

            string seleccion = await Shell.Current.DisplayActionSheet("Selecciona hora", "Cancelar", null, horas);

            if (!string.IsNullOrEmpty(seleccion) && seleccion != "Cancelar")
            {
                HoraTexto = seleccion;
                TimeSpan horaSeleccionada = TimeSpan.ParseExact(HoraTexto, @"hh\:mm", CultureInfo.InvariantCulture);
                var fechaHora = DateTime.Today.Add(horaSeleccionada);
                await _fichajeRepo.ActualizarHoraEficazAsync(999999, fechaHora);
            }
        }

        public async Task CargarHoraAsync()
        {
            try
            {
                var fichaje = await _fichajeRepo.BuscarFichajeNuevoDiaDatos();
                HoraTexto = fichaje.HoraEficaz.ToString(@"HH\:mm");
            }
            catch
            {
                HoraTexto = "—";
            }
        }

        public async Task<bool> EntradaNFCAsync()
        {
            if (!CrossNFC.Current.IsAvailable)
            {
                await Shell.Current.GoToAsync("..");
                await Shell.Current.DisplayAlert("NFC", "Este dispositivo no tiene NFC.", "OK");
                return false;
            }

            if (!CrossNFC.Current.IsEnabled)
            {
                await Shell.Current.GoToAsync("..");
                await Shell.Current.DisplayAlert("NFC", "Activa NFC en los ajustes.", "OK");
                return false;
            }

            CrossNFC.Current.OnMessageReceived -= OnTagReceived;
            CrossNFC.Current.OnMessageReceived += OnTagReceived;
            CrossNFC.Current.StartListening();
            return true;
        }

        async void OnTagReceived(ITagInfo tagInfo)
        {
            try
            {
                var serial = tagInfo.SerialNumber;
                var jornalero = await _jornaleroRepo.GetJornaleroBySerialAsync(serial);
                if (jornalero == null)
                {
                    await Shell.Current.DisplayAlert("Importante", "Jornalero no encontrado.", "OK");
                    return;
                }

                if (!TimeSpan.TryParseExact(HoraTexto, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan hora))
                {
                    await Shell.Current.DisplayAlert("Error", "Hora inválida", "OK");
                    return;
                }

                var fechaHora = DateTime.Today.Add(hora);
                var nuevoFichaje = new Fichaje
                {
                    IdJornalero = jornalero.IdJornalero,
                    HoraEficaz = fechaHora,
                    TipoFichaje = "Entrada",
                    InstanteFichaje = DateTime.Now
                };

                bool resultado = await _fichajeRepo.CrearFichajesJornalerosAsync(nuevoFichaje);

                if (resultado)
                {
                    JornalerosE.Add(new JornaleroEntrada
                    {
                        IdJornalero = jornalero.IdJornalero,
                        Nombre = jornalero.Nombre,
                        HoraEficaz = fechaHora
                    });

                    await _jornaleroRepo.SetActiveAsync(jornalero.IdJornalero, true);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Importante", "El jornalero ya fichó su entrada.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al leer NFC: {ex}");
            }
        }

        public async Task CancelarNFCAsync()
        {
            CrossNFC.Current.StopListening();
            CrossNFC.Current.OnMessageReceived -= OnTagReceived;
        }
    }
}
