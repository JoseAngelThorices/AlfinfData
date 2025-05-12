using AlfinfData.ViewModels;
using Microsoft.Maui.Controls;

namespace AlfinfData.Views.Inicio
{
    [QueryProperty(nameof(Accion), "accion")]
    public partial class DescargasPage : ContentPage
    {
        public string Accion { get; set; }

        private readonly DescargasViewModel _viewModel;

        public DescargasPage(DescargasViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Accion == "alta")
            {
                Shell.Current.FlyoutIsPresented = false;
                Accion = null; // evita que se repita
                await Task.Delay(500); // espera breve por si la interfaz aún carga
                if (_viewModel.NfcTarjetaCommand.CanExecute(null))
                    _viewModel.NfcTarjetaCommand.Execute(null);
            }
        }
    }
}
