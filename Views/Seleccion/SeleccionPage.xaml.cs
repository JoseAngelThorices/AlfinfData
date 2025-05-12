using AlfinfData.ViewModels;
using AlfinfData.Models.SQLITE;
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

        if (_viewModel.Jornaleros.Count == 0)
            await _viewModel.CargarEmpleadosAsync();

        await _viewModel.CargarCuadrillaAsync();
    }

    private void OnSeleccionarTodosClicked(object sender, EventArgs e)
    {
        _viewModel.SeleccionarTodos();
    }

    private void OnQuitarTodosClicked(object sender, EventArgs e)
    {
        _viewModel.QuitarTodos();
    }

    private async void OnJornaleroToggled(object sender, ToggledEventArgs e)
    {
        if (sender is Switch sw && sw.BindingContext is Jornalero jornalero)
        {
            await _viewModel.ActualizarJornaleroAsync(jornalero);
            _viewModel.ActualizarContador();
        }
    }

}
