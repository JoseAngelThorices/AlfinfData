using System.Collections.ObjectModel;
using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
namespace AlfinfData.ViewModels;

public class SeleccionViewModels : ContentPage
{
    private readonly JornaleroRepository _repo;
    private readonly CuadrillaRepository _repoC;

    //[ObservableProperty]
    //private Cuadrilla _cuadrillaSeleccionada;
    public ObservableCollection<Jornalero> Jornaleros { get; } = new();
    public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();
    public SeleccionViewModels(JornaleroRepository repo, CuadrillaRepository repoC)
	{
        _repo = repo;
        _repoC = repoC;
    }
    public async Task CargarEmpleadosAsync()
    {
        var lista = await _repo.GetAllAsync();
        Jornaleros.Clear();
        foreach (var j in lista)
            Jornaleros.Add(j);
    }
    public async Task CargarCuadrillaAsync()
    {
        var lista = await _repoC.GetAllAsync();
        Cuadrillas.Clear();
        foreach (var c in lista)
            Cuadrillas.Add(c);
    }

}