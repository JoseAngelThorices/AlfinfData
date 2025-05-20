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

        // Método para evitar navegación redundante y cerrar menu lateral
        private async Task NavegarSiEsNecesarioAsync(string ruta)
        {
            string rutaActual = Shell.Current.CurrentState.Location.OriginalString.ToLower();
            string rutaBase = ruta.Split('?')[0].ToLower();

            if (!rutaActual.Contains(rutaBase))
            {
                await Shell.Current.GoToAsync(ruta);
            }

            Shell.Current.FlyoutIsPresented = false;
        }

        private async void OnAccesoDirectoInicio(object sender, EventArgs e)
        {
            await NavegarSiEsNecesarioAsync("InicioPage");
        }

        private async void OnAccesoEntrada(object sender, EventArgs e)
        {
            await NavegarSiEsNecesarioAsync("EntradaPage");
        }

        private async void OnAccesoDirectoSalidas(object sender, EventArgs e)
        {
            await NavegarSiEsNecesarioAsync("SalidasPage");
        }

        private async void OnAccesoDirectoAltaNFC(object sender, EventArgs e)
        {
            await NavegarSiEsNecesarioAsync("DescargasPage?accion=alta");
        }

        private async void OnAccesoDirectoProduccion(object sender, EventArgs e)
        {
            await NavegarSiEsNecesarioAsync("ProduccionPage");
        }

        private async void OnAccesoDirectoHoras(object sender, EventArgs e)
        {
            await NavegarSiEsNecesarioAsync("HorasPage");
        }

        private async void OnAccesoDirectoFin(object sender, EventArgs e)
        {
            await NavegarSiEsNecesarioAsync("FinPage");
        }

        private async void OnAccesoDirectoSeleccion(object sender, EventArgs e)
        {
            await NavegarSiEsNecesarioAsync("SeleccionPage");
        }
    }
}
