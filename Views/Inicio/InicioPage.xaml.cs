using Microsoft.Maui.Controls;
using AlfinfData.ViewModels;
using AlfinfData.Popups;
using CommunityToolkit.Maui.Views;


namespace AlfinfData.Views.Inicio
{
    public partial class InicioPage : ContentPage
    {
        public InicioPage()
        {
            InitializeComponent();
            BindingContext = new InicioViewModel(this);
        }
        private async void OnNuevoDiaClicked(object sender, EventArgs e)
        {
            var popup = new HoraPopup();
            var resultado = await this.ShowPopupAsync(popup);

            if (resultado is TimeSpan horaSeleccionada)
            {
                var fechaHoy = DateTime.Today;
                var fechaHora = fechaHoy.Add(horaSeleccionada);

                await DisplayAlert("Nuevo Día", $"Inicio: {fechaHora:dd/MM/yyyy HH:mm}", "OK");
            }
        }
    }
}