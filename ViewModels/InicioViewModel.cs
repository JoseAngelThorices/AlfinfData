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
                    "Contrase�a requerida",
                    "Introduce la contrase�a para registrar un nuevo d�a:",
                    "Aceptar", "Cancelar", "",
                    maxLength: 10,
                    keyboard: Keyboard.Numeric);

                if (password == "123")
                {
                    await Shell.Current.GoToAsync(nameof(NuevoDiaPage));
                }
                else if (!string.IsNullOrWhiteSpace(password))
                {
                    await Shell.Current.DisplayAlert("Error", "Contrase�a incorrecta.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir Nuevo D�a", "OK");
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