using AlfinfData.Views.Configuracion;
using AlfinfData.Views.Fin;
using AlfinfData.Views.Horas;
using AlfinfData.Views.Inicio;
using AlfinfData.Views.Menu;
using AlfinfData.Views.Produccion;
using AlfinfData.Views.Salidas;
using AlfinfData.Views.Seleccion;
using AlfinfData.ViewModels;

namespace AlfinfData
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(); // Enlazamos la lógica del botón superior
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            // Paginas de INICIO
            Routing.RegisterRoute(nameof(InicioPage), typeof(InicioPage));
            Routing.RegisterRoute(nameof(EntradaPage), typeof(EntradaPage));
            Routing.RegisterRoute(nameof(DescargasPage), typeof(DescargasPage));
            Routing.RegisterRoute(nameof(NuevoDiaPage), typeof(NuevoDiaPage));

            // Paginas de HORAS
            Routing.RegisterRoute(nameof(HorasPage), typeof(HorasPage));

            // Paginas de FIN
            Routing.RegisterRoute(nameof(FinPage), typeof(FinPage));

            // Paginas de MENU
            Routing.RegisterRoute(nameof(MenuPage), typeof(MenuPage));

            // Paginas de PRODUCCION
            Routing.RegisterRoute(nameof(ProduccionPage), typeof(ProduccionPage));

            // Paginas de SALIDAS
            Routing.RegisterRoute(nameof(SalidasPage), typeof(SalidasPage));

            // Paginas de SELECCION
            Routing.RegisterRoute(nameof(SeleccionPage), typeof(SeleccionPage));

            // Paginas de CONFIGURACION
            Routing.RegisterRoute(nameof(ConfiguracionPage), typeof(ConfiguracionPage));
        }
    }
}
