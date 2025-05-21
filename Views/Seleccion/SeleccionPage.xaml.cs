using AlfinfData.Models.SQLITE;
using AlfinfData.ViewModels;

namespace AlfinfData.Views.Seleccion;

public partial class SeleccionPage : ContentPage
{
    private readonly SeleccionViewModels _viewModel;
    

    public SeleccionPage(SeleccionViewModels viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CargarCuadrillaAsync();
        await _viewModel.CargarEmpleadosAsync();
    }

    private async void OnSeleccionarTodosClicked(object sender, EventArgs e)
    {
        await _viewModel.SeleccionarTodos();
    }

    private async void OnQuitarTodosClicked(object sender, EventArgs e)
    {
        await _viewModel.QuitarTodos();
    }
    private async void activardesactivarswitch(object sender, ToggledEventArgs e)
    {
        if (sender is Switch sw && sw.BindingContext is Jornalero jornalero)
        {
            jornalero.Activo = e.Value;
            await _viewModel.ActualizarJornaleroAsync(jornalero);
            _viewModel.ActualizarContador();
        }
    }
}
