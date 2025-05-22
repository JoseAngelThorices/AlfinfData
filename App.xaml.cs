using AlfinfData.Services.BdLocal;

namespace AlfinfData
{
    public partial class App : Application
    {
        public App(DatabaseService dbService)
        {
            InitializeComponent();

            // Solución para el warning de nulabilidad al acceder a Application.Current
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }

            // Guarda el año si es la primera ejecución
            RegistrarAñoInstalacion();

            // Aquí podrías usar el repositorio fichajeRepo más adelante si lo necesitas
            var fichajeRepo = new FichajeRepository(dbService);
        }

        private void RegistrarAñoInstalacion()
        {
            if (!Preferences.ContainsKey("AñoInicio"))
            {
                Preferences.Set("AñoInicio", DateTime.Now.Year);
            }
        }

        // Este método define que la app usará AppShell como ventana raíz
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
