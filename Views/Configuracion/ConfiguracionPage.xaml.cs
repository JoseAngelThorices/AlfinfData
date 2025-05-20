using System.ComponentModel;
using AlfinfData.ViewModels;


namespace AlfinfData.Views.Configuracion
{
    public partial class ConfiguracionPage : ContentPage, INotifyPropertyChanged
    {
        //private bool _imprimirEtiqueta;
        //private bool _separadorNegro;
        //private bool _habilitarTcp;
        //private bool _enviarTotales;
        //private bool _enviarAsistencia;
        //private bool _aceptarFichajesDesconocidos;
        private readonly ConfiguracionViewModel _viewModel;
        public ConfiguracionPage(ConfiguracionViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadConfigAsync();
        }

        //private void CargarConfiguracion()
        //{
        //    //ImprimirEtiqueta = Preferences.Default.Get("ImprimirEtiqueta", false);
        //    //SeparadorNegro = Preferences.Default.Get("SeparadorNegro", false);
        //    //HabilitarTcp = Preferences.Default.Get("HabilitarTcp", false);
        //    //EnviarTotales = Preferences.Default.Get("EnviarTotales", false);
        //    //EnviarAsistencia = Preferences.Default.Get("EnviarAsistencia", false);
        //    //AceptarFichajesDesconocidos = Preferences.Default.Get("AceptarFichajesDesconocidos", false);

        //    UrlLabel.Text = Preferences.Default.Get("ServerUrl", "URL del servidor");
        //    UrlLabel.TextColor = UrlLabel.Text == "URL del servidor" ? Colors.Gray : Colors.Black;

        //    PortLabel.Text = Preferences.Default.Get("ServerPort", "Puerto del servidor");
        //    PortLabel.TextColor = PortLabel.Text == "Puerto del servidor" ? Colors.Gray : Colors.Black;


        //}
        //public new event PropertyChangedEventHandler? PropertyChanged;

        //public bool ImprimirEtiqueta
        //{
        //    get => _imprimirEtiqueta;
        //    set
        //    {
        //        if (_imprimirEtiqueta != value)
        //        {
        //            _imprimirEtiqueta = value;
        //            OnPropertyChanged();
        //            Preferences.Default.Set("ImprimirEtiqueta", value);
        //        }
        //    }
        //}

        //public bool SeparadorNegro
        //{
        //    get => _separadorNegro;
        //    set
        //    {
        //        if (_separadorNegro != value)
        //        {
        //            _separadorNegro = value;
        //            OnPropertyChanged();
        //            Preferences.Default.Set("SeparadorNegro", value);
        //        }
        //    }
        //}

        //public bool HabilitarTcp
        //{
        //    get => _habilitarTcp;
        //    set
        //    {
        //        if (_habilitarTcp != value)
        //        {
        //            _habilitarTcp = value;
        //            OnPropertyChanged();
        //            Preferences.Default.Set("HabilitarTcp", value);
        //        }
        //    }
        //}

        //public bool EnviarTotales
        //{
        //    get => _enviarTotales;
        //    set
        //    {
        //        if (_enviarTotales != value)
        //        {
        //            _enviarTotales = value;
        //            OnPropertyChanged();
        //            Preferences.Default.Set("EnviarTotales", value);
        //        }
        //    }
        //}

        //public bool EnviarAsistencia
        //{
        //    get => _enviarAsistencia;
        //    set
        //    {
        //        if (_enviarAsistencia != value)
        //        {
        //            _enviarAsistencia = value;
        //            OnPropertyChanged();
        //            Preferences.Default.Set("EnviarAsistencia", value);
        //        }
        //    }
        //}

        //public bool AceptarFichajesDesconocidos
        //{
        //    get => _aceptarFichajesDesconocidos;
        //    set
        //    {
        //        if (_aceptarFichajesDesconocidos != value)
        //        {
        //            _aceptarFichajesDesconocidos = value;
        //            OnPropertyChanged();
        //            Preferences.Default.Set("AceptarFichajesDesconocidos", value);
        //        }
        //    }
        //}

        

        //protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //private async void OnUrlClicked(object sender, EventArgs e)
        //{
        //    var result = await DisplayPromptAsync("Configurar URL", "Introduce la URL del servidor:",
        //        initialValue: UrlLabel.Text == "URL del servidor" ? "" : UrlLabel.Text);

        //    if (result != null)
        //    {
        //        UrlLabel.Text = string.IsNullOrWhiteSpace(result) ? "URL del servidor" : result;
        //        UrlLabel.TextColor = string.IsNullOrWhiteSpace(result) ? Colors.Gray : Colors.Black;
        //        Preferences.Default.Set("ServerUrl", UrlLabel.Text);
        //    }
        //}

        //private async void OnPortClicked(object sender, System.EventArgs e)
        //{
        //    var result = await DisplayPromptAsync("Configurar Puerto", "Ingrese el puerto del servidor:",
        //        initialValue: PortLabel.Text == "Puerto del servidor" ? "" : PortLabel.Text,
        //        keyboard: Keyboard.Numeric);

        //    if (result != null)
        //    {
        //        PortLabel.Text = string.IsNullOrWhiteSpace(result) ? "Puerto del servidor" : result;
        //        PortLabel.TextColor = string.IsNullOrWhiteSpace(result) ? Colors.Gray : Colors.Black;
        //        Preferences.Default.Set("ServerPort", PortLabel.Text);
        //    }
        //}   
    }
}