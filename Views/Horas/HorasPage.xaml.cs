using AlfinfData.ViewModels;
using CommunityToolkit.Maui.Views;
using AlfinfData.Models.SQLITE;

namespace AlfinfData.Views.Horas
{
    public partial class HorasPage : ContentPage
    {
        private readonly HorasViewModel _viewModel;

        public HorasPage(HorasViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCuadrillasAsync();
            await _viewModel.CargarJornalerosConHorasAsync();
        }
        private async void OnDetalleClicked(object sender, EventArgs e)
        {
            var persona = ListaHoras.SelectedItem as JornaleroConHoras;
            if (persona == null)
            {
                await DisplayAlert("Atención", "Por favor selecciona una persona de la lista.", "OK");
                return;
            }

            var popup = new Popup
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
                    new Label { Text = "Detalle Jornalero", FontAttributes = FontAttributes.Bold, FontSize = 18, HorizontalOptions = LayoutOptions.Center },
                    new Label { Text = $"ID: {persona?.IdJornalero}" },
                    new Label { Text = $"Nombre: {persona?.Nombre}" },
                    new Label { Text = $"HE1: {persona?.He1:F2}" },
                    new Label { Text = $"HE2: {persona?.He2:F2} " },
                    new Label { Text = $"Faltó: {(persona?.Falta == true ? "Sí" : "No")}" },
                    new Button
                    {
                        Text = "Cerrar",
                    }
                }
                    }
                }
            };

            await this.ShowPopupAsync(popup);
        }
    }
}
