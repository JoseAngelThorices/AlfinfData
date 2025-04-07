using AlfinfData.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace AlfinfData.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task NavigateToInicio()
        {
            await Shell.Current.GoToAsync(nameof(InicioPage));
        }

        [RelayCommand]
        private async Task NavigateToSeleccion()
        {
            await Shell.Current.GoToAsync(nameof(SeleccionPage));
        }

        [RelayCommand]
        private async Task NavigateToTareas()
        {
            await Shell.Current.GoToAsync(nameof(TareasPage));
        }

        [RelayCommand]
        private async Task NavigateToLectores()
        {
            await Shell.Current.GoToAsync(nameof(LectoresPage));
        }

        [RelayCommand]
        private async Task NavigateToProduccion()
        {
            await Shell.Current.GoToAsync(nameof(ProduccionPage));
        }

        [RelayCommand]
        private async Task NavigateToPanel()
        {
            await Shell.Current.GoToAsync(nameof(PanelFichajesPage));
        }

        [RelayCommand]
        private async Task OpenConfiguracion()
        {
            await Shell.Current.GoToAsync(nameof(ConfiguracionPage));
        }

        [RelayCommand]
        private async Task OpenMenu()
        {
            await Shell.Current.DisplayAlert("Menú", "Funcionalidad de menú lateral", "OK");
        }

        private async Task Navigate(string route)
        {
            try
            {
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo navegar: {ex.Message}", "OK");
            }
        }
    }
}
