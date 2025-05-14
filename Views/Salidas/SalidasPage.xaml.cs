using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using AlfinfData.ViewModels;


namespace AlfinfData.Views.Salidas
{
    public partial class SalidasPage : ContentPage
    {
        const string HoraGuardadaKey = "HoraSeleccionada";
        private readonly SalidasViewModel _viewModel;
        public SalidasPage(SalidasViewModel viewModel)
        {
            InitializeComponent();
            RecuperarHoraGuardada();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCuadrillasAsync();

            // Si quieres precargar los jornaleros de la primera cuadrilla (opcional):
            if (_viewModel.Cuadrillas.Any())
            {
                _viewModel.CuadrillaSeleccionada = _viewModel.Cuadrillas.First();
            }

            // 🔄 Activar NFC automáticamente al entrar
            bool iniciado = await _viewModel.SalidaNFCAsync();
            if (!iniciado)
            {
                await DisplayAlert("Error", "No se pudo iniciar la lectura NFC.", "OK");
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.CancelarNFC();
        }

        private void RecuperarHoraGuardada()
        {
            var hora = Preferences.Get(HoraGuardadaKey, string.Empty);
            if (!string.IsNullOrEmpty(hora))
            {
                HoraButton.Text = hora;
            }
        }

        private async void OnHoraButtonClicked(object sender, EventArgs e)
        {
            string[] horas = new string[48];
            for (int i = 0; i < 24; i++)
            {
                horas[i * 2] = $"{i:D2}:00";
                horas[i * 2 + 1] = $"{i:D2}:30";
            }

            string seleccion = await DisplayActionSheet("Selecciona hora", "Cancelar", null, horas);

            if (!string.IsNullOrEmpty(seleccion) && seleccion != "Cancelar")
            {
                HoraButton.Text = seleccion;
                Preferences.Set(HoraGuardadaKey, seleccion);
            }
        }
    }
}
