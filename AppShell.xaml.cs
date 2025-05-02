using AlfinfData.Views.Configuracion;
using AlfinfData.Views.Fin;
using AlfinfData.Views.Horas;
using AlfinfData.Views.Inicio;
using AlfinfData.Views.Produccion;
using AlfinfData.Views.Salidas;
using AlfinfData.Views.Seleccion;
using AlfinfData.ViewModels;
using AlfinfData.Views;

namespace AlfinfData
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(); // Enlazamos ViewModel para botón derecho
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(InicioPage), typeof(InicioPage));
            Routing.RegisterRoute(nameof(EntradaPage), typeof(EntradaPage));
            Routing.RegisterRoute(nameof(DescargasPage), typeof(DescargasPage));
            Routing.RegisterRoute(nameof(HorasPage), typeof(HorasPage));
            Routing.RegisterRoute(nameof(FinPage), typeof(FinPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage)); // <-- Corregido aquí
            Routing.RegisterRoute(nameof(ProduccionPage), typeof(ProduccionPage));
            Routing.RegisterRoute(nameof(SalidasPage), typeof(SalidasPage));
            Routing.RegisterRoute(nameof(SeleccionPage), typeof(SeleccionPage));
            Routing.RegisterRoute(nameof(ConfiguracionPage), typeof(ConfiguracionPage));
            Routing.RegisterRoute(nameof(CalcularPage), typeof(CalcularPage));
        }

        // Botón izquierdo: Volver a MainPage
        private async void OnBackToMainClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///main");
        }

        // Botón menú
        private void OnMenuClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        // Botón salir
        private void OnSalirClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
    }
}
