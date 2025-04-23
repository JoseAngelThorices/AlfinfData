using AlfinfData.ViewModels;
using AlfinfData.Services;
using System.Diagnostics;

namespace AlfinfData.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly OdooService _odoo;
        public MainPage(OdooService odoo)
        {
            InitializeComponent();
            _odoo = odoo;
            BindingContext = new MainViewModel();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Recibimos el mensaje de error, o null si todo OK
            var error = await _odoo.TryLoginAsync();

            if (error == null)
            {
                await DisplayAlert("Odoo Login", "✅ Conexión satisfactoria", "OK");
            }
            else
            {
                // Mostramos el mensaje completo de la excepción
                await DisplayAlert("Odoo Login",
                    $"❌ No se pudo conectar a Odoo:\n{error}",
                    "OK");
                Debug.WriteLine($"Login fallido: {error}");
            }
        }
    }
}
