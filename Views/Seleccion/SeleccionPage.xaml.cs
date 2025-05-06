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

        if (_viewModel.Jornaleros.Count == 0)
        {
            await _viewModel.CargarEmpleadosAsync();
        }

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
}
