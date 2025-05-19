using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Linq;
using AlfinfData.ViewModels;
using System.Collections.ObjectModel;

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

            if (_viewModel.Cuadrillas.Any())
            {
                _viewModel.CuadrillaSeleccionada = _viewModel.Cuadrillas.First();
                await _viewModel.CargarJornalerosPendientesAsync();
            }

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
