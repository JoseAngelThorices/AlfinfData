using Microsoft.Maui.Controls;

namespace AlfinfData.Views.Horas
{
    public partial class DetalleJornaleroPage : ContentPage
    {
        public DetalleJornaleroPage()
        {
            InitializeComponent();
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }


        private async void OnAceptarClicked(object sender, EventArgs e)
        {
            // Aquí puedes recoger los valores de EntryHN.Text, etc., y hacer lo que necesites
            await DisplayAlert("Guardado", "Datos guardados correctamente.", "OK");
            await Navigation.PopAsync();
        }
    }
}
