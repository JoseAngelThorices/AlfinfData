using Microsoft.Maui.Controls;
using AlfinfData.ViewModels;

namespace AlfinfData.Views.Salidas
{
    public partial class SalidasPage : ContentPage
    {
        private readonly SalidasViewModel _viewModel;

        public SalidasPage(SalidasViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.CargarCuadrillasAsync();
            await _viewModel.CargarJornalerosPendientesAsync();
            await _viewModel.GetJornaleroSalidasAsync();
            await _viewModel.SalidaNFCAsync();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await _viewModel.CancelarNFC();
        }
    }
}
