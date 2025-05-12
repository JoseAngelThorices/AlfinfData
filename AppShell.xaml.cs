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
            FechaHora = DateTime.Now.ToString("dd/MM/yyyy");
            return true;
        }

        private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
        {
            string ruta = Current?.CurrentState.Location.OriginalString?.ToLower() ?? "";
            FechaMenu = DateTime.Now.ToString("dd/MM/yyyy");

            var segments = ruta.Split('/');
            var currentPage = segments.LastOrDefault();

            Titulo = currentPage switch
            {
                "main" => "AlfinfData",
                "iniciopage" => "Inicio",
                "seleccionpage" => "Selección",
                "produccionpage" => "Producción",
                "horaspage" => "Horas",
                "salidaspage" => "Salidas",
                "finpage" => "Fin",
                "configuracionpage" => "Configuración",
                "entradapage" => "Entrada",
                "descargaspage" => "Descargas",
                _ => "AlfinfData"
            };

            if (TituloStack != null)
            {
                if (currentPage == "main")
                {
                    TituloStack.HorizontalOptions = LayoutOptions.Start;
                    TituloStack.Margin = new Thickness(10, 0, 0, 0);
                }
                else
                {
                    TituloStack.HorizontalOptions = LayoutOptions.Center;
                    TituloStack.Margin = new Thickness(0);
                }
            }
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
        private async void OnAccesoDirectoAltaNFC(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(DescargasPage)}?accion=alta");
        }

    }
}
