using AlfinfData.ViewModels;
using AlfinfData.Services.BdLocal;
using System.Collections.Generic;

namespace AlfinfData.Views.Seleccion;

public partial class SeleccionPage : ContentPage
{
    private readonly SeleccionViewModels _viewModel;
    public SeleccionPage(SeleccionViewModels viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    //Este método se realiza cuando la pagina se inicie
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Si quieres evitar recargas innecesarias: la siguiente condicion se puede descartar y solamente quedarnos con la llamada del los métodos.
        if (_viewModel.Jornaleros.Count == 0)
        {
            await _viewModel.CargarEmpleadosAsync();
        }
        await _viewModel.CargarCuadrillaAsync();
    }

}
