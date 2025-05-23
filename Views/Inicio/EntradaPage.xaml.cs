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

            // Cargar cuadrillas siempre
            await viewModel.CargarCuadrillasAsync();

            // Cargar NFC y datos si está disponible
            await viewModel.EntradaNFCAsync();
            await viewModel.CargarHoraAsync();
            await viewModel.CargarJornalerosSegunCuadrillaAsync(); // Refrescar trabajadores
            
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.CancelarNFCAsync();
        }
    }
}
