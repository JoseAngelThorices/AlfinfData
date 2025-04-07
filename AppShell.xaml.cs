using AlfinfData.Views;

namespace AlfinfData
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(InicioPage), typeof(InicioPage));
            Routing.RegisterRoute(nameof(SeleccionPage), typeof(SeleccionPage));
            Routing.RegisterRoute(nameof(TareasPage), typeof(TareasPage));
            Routing.RegisterRoute(nameof(LectoresPage), typeof(LectoresPage));
            Routing.RegisterRoute(nameof(ProduccionPage), typeof(ProduccionPage));
            Routing.RegisterRoute(nameof(PanelFichajesPage), typeof(PanelFichajesPage));
            Routing.RegisterRoute(nameof(ConfiguracionPage), typeof(ConfiguracionPage));
        }
    }
}
