using System.Diagnostics;
using AlfinfData.Popups;
using CommunityToolkit.Maui.Views;
using AlfinfData.Services.BdLocal;
using AlfinfData.Models.SQLITE;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlfinfData.ViewModels
{
    public partial class InicioViewModel : ObservableObject
    {
        private string _titulo;
        private readonly FichajeRepository _fichajeRepository;
        public string FechaSistema => $"F.T.: {DateTime.Now:dd-MM-yyyy}";
        public InicioViewModel( FichajeRepository fichajeRepository)
        {
            Titulo = "MENÚ INICIO";
            _fichajeRepository = fichajeRepository;
        }

        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                OnPropertyChanged();
            }
        }

        [RelayCommand]
        private async Task NuevoDiaAsync()
        {
            try
            {
                string password = await Shell.Current.DisplayPromptAsync(
                    "Contraseña requerida",
                    "Introduce la contraseña para registrar un nuevo día:",
                    "Aceptar", "Cancelar", "",
                    maxLength: 10,
                    keyboard: Keyboard.Numeric);

                if (password == "123")
                {
                    bool resultadoNuevoDia = await _fichajeRepository.BuscarFichajeNuevoDia();
                    if (resultadoNuevoDia)
                    {
                        bool quiereBorrar = await Shell.Current.DisplayAlert(
                                            "Nuevo Día",
                                            "Ya hay un nuevo día creado. Si quieres seguir creando un nuevo día, se borrará todo para empezar de nuevo. ¿Estás de acuerdo?",
                                            "Aceptar",   // texto del botón positivo
                                            "Cancelar"   // texto del botón negativo
                                             );
                        if (quiereBorrar)
                        {
                            // Mostrar popup para seleccionar hora
                            var popup = new HoraPopup();
                            var resultado = await Shell.Current.ShowPopupAsync(popup);
                            if (resultado is TimeSpan horaSeleccionada)
                            {
                                var fechaHoy = DateTime.Today;
                                var fechaHora = fechaHoy.Add(horaSeleccionada);

                                Titulo = $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}"; // <-- ESTA LÍNEA CAMBIA EL TÍTULO
                                // Usuario pulsó “Aceptar”
                                // Aquí borras todo y creas el nuevo día
                                var nuevoDia = new Fichaje
                                {
                                    IdJornalero = 999999,
                                    HoraEficaz = fechaHora,
                                    TipoFichaje = "Entrada",
                                    InstanteFichaje = DateTime.Today
                                };
                                
                                _fichajeRepository.BorrarDatosAsync();
                                await _fichajeRepository.CrearFichajeNuevoDiaAsync(nuevoDia);
                                await Shell.Current.DisplayAlert("Nuevo Día", $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}", "OK");
                            }
                        }
                        else
                        {
                            // Usuario pulsó “Cancelar”
                            await Shell.Current.DisplayAlert("Operación cancelada", "No se ha borrado nada.", "OK");
                        }
                    }
                    else
                    {
                        var popup = new HoraPopup();
                        var resultado = await Shell.Current.ShowPopupAsync(popup);
                        if (resultado is TimeSpan horaSeleccionada)
                        {
                            var fechaHoy = DateTime.Today;
                            var fechaHora = fechaHoy.Add(horaSeleccionada);

                            Titulo = $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}"; // <-- ESTA LÍNEA CAMBIA EL TÍTULO
                            var nuevoDia = new Fichaje
                            {
                                IdJornalero = 999999,
                                HoraEficaz = fechaHora,
                                TipoFichaje = "Entrada",
                                InstanteFichaje = DateTime.Today
                            };
                            _fichajeRepository.BorrarDatosAsync();
                            await _fichajeRepository.CrearFichajeNuevoDiaAsync(nuevoDia);
                        }
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
        [RelayCommand]
        private async Task EntradaAsync()
        {
            try
            {
                bool resultadoNuevoDia = await _fichajeRepository.BuscarFichajeNuevoDia();
                
                if (!resultadoNuevoDia)
                {
                    // 2) Si no, avisamos y salimos SIN navegar
                    await Shell.Current.DisplayAlert("Importante", "Inicie un nuevo Día primero.", "OK");
                    return;
                }
                await Shell.Current.GoToAsync(nameof(AlfinfData.Views.Inicio.EntradaPage));
                Debug.WriteLine("Navegando a EntradaPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir Entrada", "OK");
            }
        }
        [RelayCommand]
        private async Task DescargasAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(AlfinfData.Views.Inicio.DescargasPage));
                Debug.WriteLine("Navegando a DescargasPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir Descargas", "OK");
            }
        }
    }
}