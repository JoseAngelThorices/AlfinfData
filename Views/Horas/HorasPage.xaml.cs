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
            //await _viewModel.CargarJornalerosConHorasAsync();
            await _viewModel.CargarDesdeActivosAsync();
            //Guardar horas en tabla horas.
            await _viewModel.GuardarHorasAsync();


            // Temporizador que actualiza las horas automáticamente
            Dispatcher.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await _viewModel.CargarDesdeActivosAsync();
                });

                return true; // Repetir el timer
            });
        }

        

    }
}
