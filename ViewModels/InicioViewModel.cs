using System.Diagnostics;
using AlfinfData.Views.Inicio;

namespace AlfinfData.ViewModels
{
    public class InicioViewModel : BindableObject
    {
        public Command NuevoDiaCommand { get; }
        public Command EntradaCommand { get; }
        public Command DescargasCommand { get; }

        public InicioViewModel()
        {
            NuevoDiaCommand = new Command(async () => await OnNuevoDiaClicked());
            EntradaCommand = new Command(async () => await OnEntradaClicked());
            DescargasCommand = new Command(async () => await OnDescargasClicked());
        }

        private async Task OnNuevoDiaClicked()
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
                    await Shell.Current.GoToAsync(nameof(NuevoDiaPage));
                }
                else if (!string.IsNullOrWhiteSpace(password))
                {
                    await Shell.Current.DisplayAlert("Error", "Contraseña incorrecta.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir Nuevo Día", "OK");
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