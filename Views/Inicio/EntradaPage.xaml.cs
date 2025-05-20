using AlfinfData.ViewModels;

namespace AlfinfData.Views.Inicio
{
    public partial class EntradaPage : ContentPage
    {
        private readonly EntradaViewModel viewModel;

        public EntradaPage(EntradaViewModel vm)
        {
            InitializeComponent();
            viewModel = vm;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (await viewModel.EntradaNFCAsync())
            {
                await viewModel.CargarHoraAsync();
                await viewModel.CargarCuadrillasAsync(); // <- Cargar cuadrillas
                // Los jornaleros se cargarán automáticamente al seleccionar una cuadrilla
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await viewModel.CancelarNFCAsync();
        }
    }
}
