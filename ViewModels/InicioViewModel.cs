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

            // Cargar el título desde Preferences al crear el ViewModel
            CargarTituloInicio();
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
                // 1) Pedimos contraseña
                string password = await Shell.Current.DisplayPromptAsync(
                    "Contraseña requerida",
                    "Introduce la contraseña para registrar un nuevo día:",
                    "Aceptar", "Cancelar", "",
                    maxLength: 10,
                    keyboard: Keyboard.Numeric);

                if (password == "123")
                {
                    // 2) Verificamos si ya hay un día creado
                    bool resultadoNuevoDia = await _fichajeRepository.BuscarFichajeNuevoDia();

                    if (resultadoNuevoDia)
                    {
                        // 3) Confirmamos si desea borrar
                        bool quiereBorrar = await Shell.Current.DisplayAlert(
                            "Nuevo Día",
                            "Ya hay un nuevo día creado. Si quieres seguir creando un nuevo día, se borrará todo para empezar de nuevo. ¿Estás de acuerdo?",
                            "Aceptar", "Cancelar");

                        if (!quiereBorrar)
                        {
                            await Shell.Current.DisplayAlert("Operación cancelada", "No se ha borrado nada.", "OK");
                            return;
                        }
                    }

                    // 4) Mostrar popup para seleccionar hora
                    var popup = new HoraPopup();
                    var resultado = await Shell.Current.ShowPopupAsync(popup);

                    if (resultado is TimeSpan horaSeleccionada)
                    {
                        var fechaHoy = DateTime.Today;
                        var fechaHora = fechaHoy.Add(horaSeleccionada);

                        Titulo = $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}";
                        Preferences.Set("Inicio_Titulo", Titulo);
                        Preferences.Set("Inicio_Fecha", DateTime.Today.ToString("yyyy-MM-dd"));

                            var nuevoDia = new Fichaje
                            {
                                IdJornalero = 999999,
                                HoraEficaz = fechaHora,
                                TipoFichaje = "Entrada",
                                InstanteFichaje = DateTime.Today
                            };
                            await _fichajeRepository.BorrarDatosAsync();
                            await _fichajeRepository.CrearFichajeNuevoDiaAsync(nuevoDia);
                            await Shell.Current.DisplayAlert("Nuevo Día", $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}", "OK");  
                    }     

                }
                else if (!string.IsNullOrWhiteSpace(password))
                {
                    await Shell.Current.DisplayAlert("Error", "Contraseña incorrecta.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo registrar el nuevo día", "OK");
            }
        }

        // BOTÓN ENTRADA
        [RelayCommand]
        private async Task EntradaAsync()
        {
            try
            {
                bool resultadoNuevoDia = await _fichajeRepository.BuscarFichajeNuevoDia();

                if (!resultadoNuevoDia)
                {
                    await Shell.Current.DisplayAlert("Importante", "Inicie un nuevo Día primero.", "OK");
                    return;
                }

                await Shell.Current.GoToAsync(nameof(AlfinfData.Views.Inicio.EntradaPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar: {ex.Message}");
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
                Debug.WriteLine($"Error al navegar: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir Descargas", "OK");
            }
        }

        // CARGA EL TÍTULO GUARDADO SI EL DÍA ES HOY
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
