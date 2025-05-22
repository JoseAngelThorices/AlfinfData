using AlfinfData.ViewModels;
using System.Diagnostics;
using AlfinfData.Services.odoo;

namespace AlfinfData.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly OdooService _odoo;
        private readonly MainViewModel _viewModel;
        public MainPage(OdooService odoo, MainViewModel viewModel)
        {
            InitializeComponent();
            _odoo = odoo;
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();        
        }
    }
}
