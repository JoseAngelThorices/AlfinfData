using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace AlfinfData.Views.Horas
{
    public partial class HorasPage : ContentPage
    {
        public HorasPage()
        {
            InitializeComponent();

            // Datos de prueba
            var datosPrueba = new List<HorasItem>
            {
                new HorasItem { NumeroLista = 1, Nombre = "Juan", HN = 8, HE1 = 1, HE2 = 0, Falta = false },
                new HorasItem { NumeroLista = 2, Nombre = "María", HN = 7.5, HE1 = 0, HE2 = 1, Falta = true },
                new HorasItem { NumeroLista = 3, Nombre = "Carlos", HN = 8, HE1 = 0.5, HE2 = 0, Falta = false },
            };

            ListaHoras.ItemsSource = datosPrueba;
        }

        private async void OnDetalleClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DetalleJornaleroPage());
        }



        private async void OnCalcularClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CalcularPage));
        }

        // Clase auxiliar para los datos
        public class HorasItem
        {
            public int NumeroLista { get; set; }
            public string Nombre { get; set; }
            public double HN { get; set; }
            public double HE1 { get; set; }
            public double HE2 { get; set; }
            public bool Falta { get; set; }
        }
    }
}
