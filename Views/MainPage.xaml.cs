using AlfinfData.ViewModels;
using System.Diagnostics;
using AlfinfData.Services.odoo;

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

           
        }
    }
}
