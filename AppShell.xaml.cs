// AppShell.xaml.cs completamente funcional y dinámico con fecha/título
using AlfinfData.Views.Configuracion;
using AlfinfData.Views.Fin;
using AlfinfData.Views.Horas;
using AlfinfData.Views.Inicio;
using AlfinfData.Views.Produccion;
using AlfinfData.Views.Salidas;
using AlfinfData.Views.Seleccion;
using AlfinfData.Views;

namespace AlfinfData
{
    public partial class AppShell : Shell
    {
        public static readonly BindableProperty TituloProperty =
            BindableProperty.Create(nameof(Titulo), typeof(string), typeof(AppShell), "AlfinfData");

        public static readonly BindableProperty FechaHoraProperty =
            BindableProperty.Create(nameof(FechaHora), typeof(string), typeof(AppShell), DateTime.Now.ToString("dd/MM/yyyy"));

        public static readonly BindableProperty FechaMenuProperty =
            BindableProperty.Create(nameof(FechaMenu), typeof(string), typeof(AppShell), DateTime.Now.ToString("dd/MM/yyyy"));

        public string Titulo
        {
            get => (string)GetValue(TituloProperty);
            set => SetValue(TituloProperty, value);
        }

        public string FechaHora
        {
            get => (string)GetValue(FechaHoraProperty);
            set => SetValue(FechaHoraProperty, value);
        }

        public string FechaMenu
        {
            get => (string)GetValue(FechaMenuProperty);
            set => SetValue(FechaMenuProperty, value);
        }

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
            RegisterRoutes();
            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), ActualizarFechaHora);
            Navigated += OnShellNavigated;
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(InicioPage), typeof(InicioPage));
            Routing.RegisterRoute(nameof(EntradaPage), typeof(EntradaPage));
            Routing.RegisterRoute(nameof(DescargasPage), typeof(DescargasPage));
            Routing.RegisterRoute(nameof(HorasPage), typeof(HorasPage));
            Routing.RegisterRoute(nameof(FinPage), typeof(FinPage));
            Routing.RegisterRoute(nameof(ProduccionPage), typeof(ProduccionPage));
            Routing.RegisterRoute(nameof(SalidasPage), typeof(SalidasPage));
            Routing.RegisterRoute(nameof(SeleccionPage), typeof(SeleccionPage));
            Routing.RegisterRoute(nameof(ConfiguracionPage), typeof(ConfiguracionPage));
            Routing.RegisterRoute(nameof(CalcularPage), typeof(CalcularPage));
        }

        private async void OnBackToMainClicked(object sender, EventArgs e)
        {
            await GoToAsync("///main");
        }

        private void OnSalirClicked(object sender, EventArgs e)
        {
            Application.Current?.Quit();
        }

        private bool ActualizarFechaHora()
        {
            var ahora = DateTime.Now;
            string ruta = Current?.CurrentState.Location.OriginalString?.ToLower() ?? "";

            FechaHora = ahora.ToString("dd/MM/yyyy");

            return true;
        }

        private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
        {
            string ruta = Current?.CurrentState.Location.OriginalString?.ToLower() ?? "";
            FechaMenu = DateTime.Now.ToString("dd/MM/yyyy");

            if (ruta.Contains("main")) Titulo = "AlfinfData";
            else if (ruta.Contains("inicio")) Titulo = "Inicio";
            else if (ruta.Contains("seleccion")) Titulo = "Selección";
            else if (ruta.Contains("produccion")) Titulo = "Producción";
            else if (ruta.Contains("horas")) Titulo = "Horas";
            else if (ruta.Contains("salidas")) Titulo = "Salidas";
            else if (ruta.Contains("fin")) Titulo = "Fin";
            else if (ruta.Contains("configuracion")) Titulo = "Configuración";
            else Titulo = "AlfinfData";
        }

        private async void OnAbrirConfiguracionClicked(object sender, EventArgs e)
        {
            string? password = await Application.Current.MainPage.DisplayPromptAsync(
                "Contraseña",
                "Introduce la contraseña para acceder a Configuración:",
                accept: "Entrar",
                cancel: "Cancelar",
                placeholder: "",
                maxLength: 10,
                keyboard: Keyboard.Text);

            if (password == "123")
            {
                await Shell.Current.GoToAsync(nameof(ConfiguracionPage));
            }
            else if (!string.IsNullOrWhiteSpace(password))
            {
                await Application.Current.MainPage.DisplayAlert("Acceso denegado", "Contraseña incorrecta.", "OK");
            }
        }
    }
}
