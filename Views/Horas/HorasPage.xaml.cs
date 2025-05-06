using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;

namespace AlfinfData.Views.Horas
{
    public partial class HorasPage : ContentPage
    {
        // Persona seleccionada en la lista
        private HorasItem personaSeleccionada;

        public HorasPage()
        {
            InitializeComponent();

            // Datos de prueba
            var datosPrueba = new List<HorasItem>
            {
                new HorasItem { NumeroLista = 1, Nombre = "Juan", HN = 8, HE1 = 1, HE2 = 0, Falta = false },
                new HorasItem { NumeroLista = 2, Nombre = "María", HN = 7.5, HE1 = 0, HE2 = 1, Falta = false },
                new HorasItem { NumeroLista = 3, Nombre = "Carlos", HN = 8, HE1 = 0.5, HE2 = 0, Falta = false },
            };

            ListaHoras.ItemsSource = datosPrueba;
        }

        // Evento cuando el usuario selecciona una persona de la lista
        private void OnPersonaSeleccionada(object sender, SelectionChangedEventArgs e)
        {
            personaSeleccionada = e.CurrentSelection.FirstOrDefault() as HorasItem;
        }



        private async void OnDetalleClicked(object sender, EventArgs e)
{
    var persona = ListaHoras.SelectedItem as HorasItem;

    if (persona == null)
    {
        await DisplayAlert("Atención", "Por favor selecciona una persona de la lista.", "OK");
        return;
    }

    Popup popup = null;

    var cerrarButton = new Button
    {
        Text = "Cerrar",
        HorizontalOptions = LayoutOptions.Center,
        Command = new Command(() => popup?.Close())
    };

    popup = new Popup
    {
        CanBeDismissedByTappingOutsideOfPopup = true,
        Content = new Frame
        {
            Padding = 20,
            CornerRadius = 10,
            BackgroundColor = Colors.White,
            MinimumWidthRequest = 300,
            MinimumHeightRequest = 300,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        Text = "Detalle Jornalero",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label { Text = $"Nombre: {persona.Nombre}" },
                    new Label { Text = $"Nº Lista: {persona.NumeroLista}" },
                    new Label { Text = $"HN: {persona.HN}" },
                    new Label { Text = $"HE1: {persona.HE1}" },
                    new Label { Text = $"HE2: {persona.HE2}" },
                    new Label { Text = $"Faltó: {(persona.Falta ? "Sí" : "No")}" },
                    cerrarButton
                }
            }
        }
    };

    await this.ShowPopupAsync(popup);
}


        // Botón de "Calcular"
        private async void OnCalcularClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CalcularPage));
        }

        // Clase de datos
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
