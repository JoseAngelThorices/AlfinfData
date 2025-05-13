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
    }
}
