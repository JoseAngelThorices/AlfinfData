using System.Diagnostics;
using AlfinfData.Popups;
using CommunityToolkit.Maui.Views;


namespace AlfinfData.ViewModels
{
    public class InicioViewModel : BindableObject
    {
        private readonly Page _page;

        private string _titulo;
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                OnPropertyChanged();
            }
        }

        public string FechaSistema => $"F.T.: {DateTime.Now:dd-MM-yyyy}";

        public Command NuevoDiaCommand { get; }
        public Command EntradaCommand { get; }
        public Command DescargasCommand { get; }

        public InicioViewModel(Page page)
        {
            _page = page;

            Titulo = "MENÚ INICIO";

            NuevoDiaCommand = new Command(async () => await OnNuevoDiaClicked());
            EntradaCommand = new Command(async () => await OnEntradaClicked());
            DescargasCommand = new Command(async () => await OnDescargasClicked());
        }


        private async Task OnNuevoDiaClicked()
        {
            try
            {
                string password = await _page.DisplayPromptAsync(
                    "Contraseña requerida",
                    "Introduce la contraseña para registrar un nuevo día:",
                    "Aceptar", "Cancelar", "",
                    maxLength: 10,
                    keyboard: Keyboard.Numeric);

                if (password == "123")
                {
                    // Mostrar popup para seleccionar hora
                    var popup = new HoraPopup();
                    var resultado = await _page.ShowPopupAsync(popup);

                    if (resultado is TimeSpan horaSeleccionada)
                    {
                        var fechaHoy = DateTime.Today;
                        var fechaHora = fechaHoy.Add(horaSeleccionada);

                        Titulo = $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}"; // <-- ESTA LÍNEA CAMBIA EL TÍTULO

                        await _page.DisplayAlert("Nuevo Día", $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}", "OK");

                    }
                }
                else if (!string.IsNullOrWhiteSpace(password))
                {
                    await _page.DisplayAlert("Error", "Contraseña incorrecta.", "OK");
                }
            }
            catch (Exception ex)
            {
                await _page.DisplayAlert("Error", "No se pudo registrar el nuevo día", "OK");
            }
        }

        private async Task OnEntradaClicked()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(AlfinfData.Views.Inicio.EntradaPage));
                Debug.WriteLine("Navegando a EntradaPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir Entrada", "OK");
            }
        }

        private async Task OnDescargasClicked()
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