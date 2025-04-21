using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace AlfinfData.Views.Configuracion
{
    public partial class ConfiguracionPage : ContentPage, INotifyPropertyChanged
    {
        private bool _imprimirEtiqueta;
        private bool _separadorNegro;
        private bool _habilitarTcp;
        private bool _enviarTotales;
        private bool _enviarAsistencia;
        private bool _aceptarFichajesDesconocidos;

        public new event PropertyChangedEventHandler? PropertyChanged;

        public bool ImprimirEtiqueta
        {
            get => _imprimirEtiqueta;
            set
            {
                if (_imprimirEtiqueta != value)
                {
                    _imprimirEtiqueta = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("ImprimirEtiqueta", value);
                }
            }
        }

        public bool SeparadorNegro
        {
            get => _separadorNegro;
            set
            {
                if (_separadorNegro != value)
                {
                    _separadorNegro = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("SeparadorNegro", value);
                }
            }
        }

        public bool HabilitarTcp
        {
            get => _habilitarTcp;
            set
            {
                if (_habilitarTcp != value)
                {
                    _habilitarTcp = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("HabilitarTcp", value);
                }
            }
        }

        public bool EnviarTotales
        {
            get => _enviarTotales;
            set
            {
                if (_enviarTotales != value)
                {
                    _enviarTotales = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("EnviarTotales", value);
                }
            }
        }

        public bool EnviarAsistencia
        {
            get => _enviarAsistencia;
            set
            {
                if (_enviarAsistencia != value)
                {
                    _enviarAsistencia = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("EnviarAsistencia", value);
                }
            }
        }

        public bool AceptarFichajesDesconocidos
        {
            get => _aceptarFichajesDesconocidos;
            set
            {
                if (_aceptarFichajesDesconocidos != value)
                {
                    _aceptarFichajesDesconocidos = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("AceptarFichajesDesconocidos", value);
                }
            }
        }

        public ConfiguracionPage()
        {
            InitializeComponent();
            BindingContext = this;
            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            ImprimirEtiqueta = Preferences.Default.Get("ImprimirEtiqueta", false);
            SeparadorNegro = Preferences.Default.Get("SeparadorNegro", false);
            HabilitarTcp = Preferences.Default.Get("HabilitarTcp", false);
            EnviarTotales = Preferences.Default.Get("EnviarTotales", false);
            EnviarAsistencia = Preferences.Default.Get("EnviarAsistencia", false);
            AceptarFichajesDesconocidos = Preferences.Default.Get("AceptarFichajesDesconocidos", false);

            IpLabel.Text = Preferences.Default.Get("ServerIp", "IP del servidor");
            IpLabel.TextColor = IpLabel.Text == "IP del servidor" ? Colors.Gray : Colors.Black;

            PortLabel.Text = Preferences.Default.Get("ServerPort", "Puerto del servidor");
            PortLabel.TextColor = PortLabel.Text == "Puerto del servidor" ? Colors.Gray : Colors.Black;

            MacLabel.Text = Preferences.Default.Get("PrinterMac", "Dirección MAC de la impresora");
            MacLabel.TextColor = MacLabel.Text == "Dirección MAC de la impresora" ? Colors.Gray : Colors.Black;

            IdEntradaLabel.Text = Preferences.Default.Get("EntryId", "ID para la siguiente entrada");
            IdEntradaLabel.TextColor = IdEntradaLabel.Text == "ID para la siguiente entrada" ? Colors.Gray : Colors.Black;
        }

        protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void OnIpClicked(object sender, System.EventArgs e)
        {
            var result = await DisplayPromptAsync("Configurar IP", "Ingrese la dirección IP del servidor:",
                initialValue: IpLabel.Text == "IP del servidor" ? "" : IpLabel.Text);

            if (result != null)
            {
                IpLabel.Text = string.IsNullOrWhiteSpace(result) ? "IP del servidor" : result;
                IpLabel.TextColor = string.IsNullOrWhiteSpace(result) ? Colors.Gray : Colors.Black;
                Preferences.Default.Set("ServerIp", IpLabel.Text);
            }
        }

        private async void OnPortClicked(object sender, System.EventArgs e)
        {
            var result = await DisplayPromptAsync("Configurar Puerto", "Ingrese el puerto del servidor:",
                initialValue: PortLabel.Text == "Puerto del servidor" ? "" : PortLabel.Text,
                keyboard: Keyboard.Numeric);

            if (result != null)
            {
                PortLabel.Text = string.IsNullOrWhiteSpace(result) ? "Puerto del servidor" : result;
                PortLabel.TextColor = string.IsNullOrWhiteSpace(result) ? Colors.Gray : Colors.Black;
                Preferences.Default.Set("ServerPort", PortLabel.Text);
            }
        }

        private async void OnMacClicked(object sender, System.EventArgs e)
        {
            var result = await DisplayPromptAsync("Configurar MAC", "Ingrese la dirección MAC de la impresora:",
                initialValue: MacLabel.Text == "Dirección MAC de la impresora" ? "" : MacLabel.Text);

            if (result != null)
            {
                MacLabel.Text = string.IsNullOrWhiteSpace(result) ? "Dirección MAC de la impresora" : result;
                MacLabel.TextColor = string.IsNullOrWhiteSpace(result) ? Colors.Gray : Colors.Black;
                Preferences.Default.Set("PrinterMac", MacLabel.Text);
            }
        }

        private async void OnIdEntradaClicked(object sender, System.EventArgs e)
        {
            var result = await DisplayPromptAsync("Configurar ID Entrada", "Ingrese el ID para la siguiente entrada:",
                initialValue: IdEntradaLabel.Text == "ID para la siguiente entrada" ? "" : IdEntradaLabel.Text,
                keyboard: Keyboard.Numeric);

            if (result != null)
            {
                IdEntradaLabel.Text = string.IsNullOrWhiteSpace(result) ? "ID para la siguiente entrada" : result;
                IdEntradaLabel.TextColor = string.IsNullOrWhiteSpace(result) ? Colors.Gray : Colors.Black;
                Preferences.Default.Set("EntryId", IdEntradaLabel.Text);
            }
        }
    }
}