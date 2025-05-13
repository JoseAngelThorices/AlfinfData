using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlfinfData.Services.BdLocal;
using AlfinfData.Models.SQLITE;
using System.Diagnostics;
using Plugin.NFC;
using System.Globalization;

namespace AlfinfData.ViewModels
{
    public partial class EntradaViewModel : ObservableObject
    {
        private readonly FichajeRepository _fichajeRepo;
        private readonly JornaleroRepository _jornalerojeRepo;
        public ObservableCollection<JornaleroEntrada> JornalerosE { get; set; }  = new ObservableCollection<JornaleroEntrada>();

        [ObservableProperty]
        private string _horaTexto = "HORA";
        public EntradaViewModel(FichajeRepository fichajeRepo, JornaleroRepository jornalerojeRepo) 
        {
            _fichajeRepo = fichajeRepo;
            _jornalerojeRepo = jornalerojeRepo;
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
                HoraTexto = seleccion.ToString();
                TimeSpan horaSeleccionada = TimeSpan.ParseExact(HoraTexto, @"hh\:mm", CultureInfo.InvariantCulture);
                var fechaHoy = DateTime.Today;
                var fechaHora = fechaHoy.Add(horaSeleccionada);
                await _fichajeRepo.ActualizarHoraEficazAsync(999999, fechaHora);
                
            }
        }
        public async Task CargarFichajeAsync()
        {
            var lista = await _fichajeRepo.GetJornaleroEntradasAsync();
            if (lista != null)
            {
                JornalerosE.Clear();
                foreach (var e in lista)
                {           
                    JornalerosE.Add(e);                   
               }
            }

        }
        public async Task<bool> EntradaNFCAsync()
        {
            // Comprueba que el usuario lo tenga activado
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
            CrossNFC.Current.OnMessageReceived += OnTagReceived;
            try
            {            
                CrossNFC.Current.StartListening();
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error NFC", ex.Message, "OK");
                return false; 
            }
        }
        async void OnTagReceived(ITagInfo tagInfo)
        {
            try
            {
                var idBytes = tagInfo.Identifier;
                var idHex = BitConverter.ToString(idBytes);

                // serial como string
                var serial = tagInfo.SerialNumber;
                var jornalero = await _jornalerojeRepo.GetJornaleroBySerialAsync(serial);
                if (jornalero != null)
                {
                    TimeSpan hora = TimeSpan.ParseExact(
                        HoraTexto,
                        @"hh\:mm",
                        CultureInfo.InvariantCulture
                    );
                    var fechaHoy = DateTime.Today;
                    var fechaHora = fechaHoy.Add(hora);
                    var nuevoFichaje = new Fichaje
                    {
                        IdJornalero = jornalero.IdJornalero,
                        HoraEficaz = fechaHora,
                        TipoFichaje = "Entrada",
                        InstanteFichaje = DateTime.Today
                    };

                    bool resultado = await _fichajeRepo.CrearFichajesAsync(nuevoFichaje);

                    if (resultado)
                    {
                        var JornaleroEntrando = new JornaleroEntrada
                        {
                            IdJornalero = jornalero.IdJornalero,
                            Nombre = jornalero.Nombre,
                            HoraEficaz = fechaHora
                        };
                        JornalerosE.Add(JornaleroEntrando);
                        await _jornalerojeRepo.SetActiveAsync(jornalero.IdJornalero, true);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Importante", "El jornalero ya fichó su entrada.", "OK");
                    }
                    
                }
                else
                {
                    await Shell.Current.DisplayAlert("Importante", "No se encontró ningún jornalero con ese serial. Debería de descargar de nuevo los jornaleros.", "OK");
                    
                }
            }
            catch (FormatException fe)
            {
                Debug.WriteLine($"Error de formato al parsear HoraTexto: {fe}");
                // Opcional: notificar al usuario, asignar valor por defecto, etc.
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al procesar el tag NFC: {ex}");
                // Opcional: manejo genérico de errores
            }


        }
        public async Task cancelarNFC()
        {
            CrossNFC.Current.StopListening();
            CrossNFC.Current.OnMessageReceived -= OnTagReceived;
        }
        public async Task CargarHoraAsync()
        {
            
             try
            {
                // Asegúrate de que el repositorio esté inicializado
                if (_fichajeRepo == null)
                    throw new InvalidOperationException("_fichajeRepo no está inicializado");

                // Obtén la lista (puede devolver null)
                var fichaje = await _fichajeRepo.GetFirstByJornaleroAsync(999999);
  
                // Supongamos que Hora es TimeSpan
                HoraTexto = fichaje.HoraEficaz.ToString(@"HH\:mm");
                
            }
            catch (NullReferenceException nre)
            {
                // Captura explicita de NRE
                Debug.WriteLine($"NullReferenceException en CargarHoraAsync: {nre}");
                HoraTexto = "—";
                
            }
            catch (Exception ex)
            {
                // Cualquier otra excepción
                Debug.WriteLine($"Error en CargarHoraAsync: {ex}");
                HoraTexto = "Error";
               
            }

        }
    }
}
