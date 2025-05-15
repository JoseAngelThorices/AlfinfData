using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.NFC;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace AlfinfData.ViewModels
{
    public partial class SalidasViewModel : ObservableObject
    {
        private readonly FichajeRepository _fichajeRepo;
        private readonly JornaleroRepository _jornaleroRepo;
        private readonly CuadrillaRepository _cuadrillaRepo;

        public ObservableCollection<JornaleroEntrada> JornalerosE { get; set; } = new();
        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();
        public ObservableCollection<Jornalero> JornalerosPendientes { get; } = new();

        [ObservableProperty]
        private Cuadrilla cuadrillaSeleccionada;

        [ObservableProperty]
        private string _horaTexto = "HORA";

        public SalidasViewModel(FichajeRepository fichajeRepo, JornaleroRepository jornaleroRepo, CuadrillaRepository cuadrillaRepo)
        {
            _fichajeRepo = fichajeRepo;
            _jornaleroRepo = jornaleroRepo;
            _cuadrillaRepo = cuadrillaRepo;
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

        [RelayCommand]
        public async Task<bool> SalidaNFCAsync()
        {
            if (!CrossNFC.Current.IsAvailable)
            {
                await Shell.Current.DisplayAlert("NFC", "Este dispositivo no tiene NFC.", "OK");
                return false;
            }

            if (!CrossNFC.Current.IsEnabled)
            {
                await Shell.Current.DisplayAlert("NFC", "Activa NFC en los ajustes.", "OK");
                return false;
            }

            try
            {
                CrossNFC.Current.OnMessageReceived -= OnTagReceived;
                CrossNFC.Current.OnMessageReceived += OnTagReceived;
                CrossNFC.Current.StartListening();
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error NFC", ex.Message, "OK");
                return false;
            }
        }

        private async void OnTagReceived(ITagInfo tagInfo)
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

                if (jornalero.Activo != true)
                {
                    await Shell.Current.DisplayAlert("Atención", "El jornalero ya fichó su salida.", "OK");
                    return;
                }

                var hayInicioDia = await _fichajeRepo.BuscarFichajeNuevoDia();
                if (!hayInicioDia)
                {
                    await Shell.Current.DisplayAlert("Importante", "Inicia el día primero.", "OK");
                    return;
                }

                if (!TimeSpan.TryParseExact(HoraTexto, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan hora))
                {
                    await Shell.Current.DisplayAlert("Error", "Selecciona una hora válida antes de fichar.", "OK");
                    return;
                }

                var fechaHora = DateTime.Today.Add(hora);

                var nuevoFichaje = new Fichaje
                {
                    IdJornalero = jornalero.IdJornalero,
                    HoraEficaz = fechaHora,
                    TipoFichaje = "Salida",
                    InstanteFichaje = DateTime.Now
                };
                await _fichajeRepo.CrearFichajesJornalerosAsync(nuevoFichaje); // no importa si devuelve false
                await _jornaleroRepo.SetActiveAsync(jornalero.IdJornalero, false); // desactiva al fichar

                JornalerosE.Add(new JornaleroEntrada
                {
                    IdJornalero = jornalero.IdJornalero,
                    Nombre = jornalero.Nombre,
                    HoraEficaz = fechaHora
                });

                var pendiente = JornalerosPendientes.FirstOrDefault(j => j.IdJornalero == jornalero.IdJornalero);
                if (pendiente != null)
                    JornalerosPendientes.Remove(pendiente);

                await Shell.Current.DisplayAlert("Correcto", $"{jornalero.Nombre} fichó salida a las {fechaHora:HH:mm}.", "OK");

            }
            catch (FormatException fe)
            {
                Debug.WriteLine($"Error de formato en HoraTexto: {fe}");
                await Shell.Current.DisplayAlert("Error", "Formato de hora incorrecto", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al procesar NFC: {ex}");
                await Shell.Current.DisplayAlert("Error", "Error general de lectura NFC", "OK");
            }
        }


        public async Task CargarCuadrillasAsync()
        {
            Cuadrillas.Clear();
            var lista = await _cuadrillaRepo.GetAllAsync();
            foreach (var c in lista)
                Cuadrillas.Add(c);
        }

        public async Task CargarJornalerosPendientesAsync()
        {
            JornalerosPendientes.Clear();

            if (CuadrillaSeleccionada == null)
                return;

            var lista = await _jornaleroRepo.GetJornalerosActivosPorCuadrillaAsync(CuadrillaSeleccionada.IdCuadrilla);
            var idsExistentes = JornalerosPendientes.Select(j => j.IdJornalero).ToHashSet();

            foreach (var j in lista)
            {
                if (!idsExistentes.Contains(j.IdJornalero))
                    JornalerosPendientes.Add(j);
            }
        }
        public async Task GetJornaleroSalidasAsync()
        {
            var lista = await _fichajeRepo.GetJornaleroSalidasAsync();

            if (lista != null)
            {
                JornalerosE.Clear();
                foreach (var e in lista)
                {
                    JornalerosE.Add(e);
                }
            }
        }

        [RelayCommand]
        public async Task SeleccionarHoraAsync()
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
                Preferences.Set("HoraSeleccionada", seleccion);
            }
        }

        public async Task CancelarNFC()
        {
            CrossNFC.Current.StopListening();
            CrossNFC.Current.OnMessageReceived -= OnTagReceived;
        }



        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla value)
        {
            _ = CargarJornalerosPendientesAsync();
        }
    }
}