using Microsoft.Maui.Controls;
using System;

namespace AlfinfData.Views.BarraLateral
{
    public partial class BarraLateral : VerticalStackLayout
    {
        public BarraLateral()
        {
            InitializeComponent();
            BindingContext = Shell.Current;
        }

        private async void OnAccesoDirectoAltaNFC(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("DescargasPage?accion=alta");
        }

        private async void OnAccesoEntrada(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("EntradaPage");
        }

        private async void OnAccesoDirectoInicio(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("InicioPage");
        }

        private async void OnAccesoDirectoProduccion(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ProduccionPage");
        }

        private async void OnAccesoDirectoSeleccion(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SeleccionPage");
        }

        private async void OnAccesoDirectoHoras(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("HorasPage");
        }

        private async void OnAccesoDirectoFin(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("FinPage");
        }

        private async void OnAccesoDirectoSalidas(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SalidasPage");
        }

    }
}
