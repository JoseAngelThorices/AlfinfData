using AlfinfData.Views.Configuracion;
using AlfinfData.Views.Fin;
using AlfinfData.Views.Horas;
using AlfinfData.Views.Inicio;
using AlfinfData.Views.Menu;
using AlfinfData.Views.Produccion;
using AlfinfData.Views.Salidas;
using AlfinfData.Views.Seleccion;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlfinfData.ViewModels;


namespace AlfinfData.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SalidasViewModel vm;

        public MainViewModel(SalidasViewModel vmo)
        {
 
            vm = vmo;
        }

        [RelayCommand]
        private async Task NavigateToInicio() => await Shell.Current.GoToAsync(nameof(InicioPage));

        [RelayCommand]
        private async Task NavigateToSeleccion() => await Shell.Current.GoToAsync(nameof(SeleccionPage));

        [RelayCommand]
        private async Task NavigateToSalidas()
        {
            if (await vm.ComprobacionNFCAsync())
            {
                await Shell.Current.GoToAsync(nameof(SalidasPage));
            }
        }

        [RelayCommand]
        private async Task NavigateToHoras() => await Shell.Current.GoToAsync(nameof(HorasPage));

        [RelayCommand]
        private async Task NavigateToProduccion() => await Shell.Current.GoToAsync(nameof(ProduccionPage));

        [RelayCommand]
        private async Task NavigateToPanel() => await Shell.Current.GoToAsync(nameof(FinPage));

        [RelayCommand]
        private async Task OpenMenu() => await Shell.Current.GoToAsync(nameof(MenuPage));

        [RelayCommand]
        private async Task OpenConfiguracion()
        {
            await Shell.Current.GoToAsync(nameof(ConfiguracionPage));
        }

        [RelayCommand]
        private async Task OpenConfiguracionMenu()
        {
            string action = await Shell.Current.DisplayActionSheet(
                "Opciones", "Cancelar", null, "Configuración");

            if (action == "Configuración")
            {
                string password = await Shell.Current.DisplayPromptAsync(
                    "Contraseña requerida",
                    "Introduce la contraseña para acceder a la configuración:",
                    "Aceptar", "Cancelar", "",
                    maxLength: 10,
                    keyboard: Keyboard.Numeric);

                if (password == "123")
                {
                    await Shell.Current.GoToAsync(nameof(ConfiguracionPage));
                }
                else if (!string.IsNullOrWhiteSpace(password))
                {
                    await Shell.Current.DisplayAlert("Error", "Contraseña incorrecta.", "OK");
                }
            }
        }
    }
}
