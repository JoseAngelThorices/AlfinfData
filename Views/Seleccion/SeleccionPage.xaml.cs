using AlfinfData.Models;
using AlfinfData.Services;
using System.Collections.Generic;
using AlfinfData.ViewModels;

namespace AlfinfData.Views.Seleccion;

public partial class SeleccionPage : ContentPage
{
    //private DatabaseService _dbService = new();
    private readonly EmpleadosViewModel _vm;
    public SeleccionPage()
    {
        InitializeComponent();
      
        //CargarCuadrillas();
    }
    

    //private async void CargarCuadrillas()
    //{
    //    await _dbService.InitAsync();
    //    await _dbService.InsertarCuadrillasDePruebaAsync();

    //    var cuadrillas = await _dbService.GetCuadrillasAsync();
    //    GrupoPicker.ItemsSource = cuadrillas;
    //    GrupoPicker.ItemDisplayBinding = new Binding("Descripcion");
    //}
}
