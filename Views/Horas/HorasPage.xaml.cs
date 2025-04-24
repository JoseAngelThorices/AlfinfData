using Microsoft.Maui.Controls;

namespace AlfinfData.Views.Horas
{
    public partial class HorasPage : ContentPage
    {
        public HorasPage()
        {
            InitializeComponent();
        }

        private async void OnCalcularClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CalcularPage));
        }
    }
}
