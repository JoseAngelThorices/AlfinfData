using System.Diagnostics;
using AlfinfData.Popups;
using CommunityToolkit.Maui.Views;
using AlfinfData.Services.BdLocal;
using AlfinfData.Models.SQLITE;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;

namespace AlfinfData.ViewModels
{
    public partial class InicioViewModel : ObservableObject
    {
        private string _titulo;
        private readonly FichajeRepository _fichajeRepository;

        public string FechaSistema => $"F.T.: {DateTime.Now:dd-MM-yyyy}";

        public InicioViewModel(FichajeRepository fichajeRepository)
        {
            Titulo = "MENÚ INICIO";
            _fichajeRepository = fichajeRepository;
            CargarTituloInicio(); // Carga título si ya existe para hoy
        }

        public string Titulo
        {
            get => _titulo;
            set => SetProperty(ref _titulo, value);
        }

        // BOTÓN NUEVO DÍA
        [RelayCommand]
        private async Task NuevoDiaAsync()
        {
            try
            {
                // 1) Solicita contraseña al usuario
                string password = await Shell.Current.DisplayPromptAsync(
                    "Contraseña requerida",
                    "Introduce la contraseña para registrar un nuevo día:",
                    "Aceptar", "Cancelar", "",
                    maxLength: 10,
                    keyboard: Keyboard.Numeric);

                if (password == "123")
                {
                    // 2) Verifica si ya hay un fichaje de nuevo día
                    bool yaExiste = await _fichajeRepository.BuscarFichajeNuevoDia();

                    if (yaExiste)
                    {
                        // 3) Pregunta si desea borrar los datos actuales
                        bool quiereBorrar = await Shell.Current.DisplayAlert(
                            "Nuevo Día",
                            "Ya hay un nuevo día creado. Si continúas, se borrarán todos los datos anteriores. ¿Deseas continuar?",
                            "Aceptar", "Cancelar");

                        if (!quiereBorrar)
                        {
                            await Shell.Current.DisplayAlert("Cancelado", "No se ha borrado nada.", "OK");
                            return;
                        }
                    }

                    // 4) Muestra popup para seleccionar la hora de inicio
                    var popup = new HoraPopup();
                    var resultado = await Shell.Current.ShowPopupAsync(popup);

                    if (resultado is TimeSpan horaSeleccionada)
                    {
                        var fechaHora = DateTime.Today.Add(horaSeleccionada);

                        // Guarda título y fecha en Preferences
                        Titulo = $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}";
                        Preferences.Set("Inicio_Titulo", Titulo);
                        Preferences.Set("Inicio_Fecha", DateTime.Today.ToString("yyyy-MM-dd"));

                        var nuevoFichaje = new Fichaje
                        {
                            IdJornalero = 999999,
                            HoraEficaz = fechaHora,
                            TipoFichaje = "Entrada",
                            InstanteFichaje = DateTime.Today
                        };

                        await _fichajeRepository.BorrarDatosAsync();
                        await _fichajeRepository.CrearFichajeNuevoDiaAsync(nuevoFichaje);

                        await Shell.Current.DisplayAlert("Nuevo Día", $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}", "OK");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(password))
                {
                    await Shell.Current.DisplayAlert("Error", "Contraseña incorrecta.", "OK");
                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo registrar el nuevo día.", "OK");
            }
        }

        // BOTÓN ENTRADA
        [RelayCommand]
        private async Task EntradaAsync()
        {
            try
            {
                bool nuevoDiaExiste = await _fichajeRepository.BuscarFichajeNuevoDia();

                if (!nuevoDiaExiste)
                {
                    await Shell.Current.DisplayAlert("Importante", "Inicie un nuevo día primero.", "OK");
                    return;
                }

                await Shell.Current.GoToAsync(nameof(AlfinfData.Views.Inicio.EntradaPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar a Entrada: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir Entrada", "OK");
            }
        }

        // BOTÓN DESCARGAS
        [RelayCommand]
        private async Task DescargasAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(AlfinfData.Views.Inicio.DescargasPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar a Descargas: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir Descargas", "OK");
            }
        }

        // Recupera el título si la fecha guardada es la de hoy
        public void CargarTituloInicio()
        {
            string fechaGuardada = Preferences.Get("Inicio_Fecha", "");
            string tituloGuardado = Preferences.Get("Inicio_Titulo", "MENÚ INICIO");

            if (DateTime.TryParse(fechaGuardada, out var fecha) && fecha.Date == DateTime.Today)
            {
                Titulo = tituloGuardado;
            }
            else
            {
                Titulo = "MENÚ INICIO";
            }
        }
    }
}
