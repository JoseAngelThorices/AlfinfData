using System.Collections.ObjectModel;
using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
namespace AlfinfData.ViewModels;

public class SeleccionViewModels : ContentPage
{
    private readonly JornaleroRepository _repo;

    public ObservableCollection<Jornalero> Jornaleros { get; } = new();
    public SeleccionViewModels(JornaleroRepository repo)
	{
        _repo = repo;
    }
    public async Task CargarEmpleadosAsync()
    {
        var lista = await _repo.GetAllAsync();
        Jornaleros.Clear();
        foreach (var j in lista)
            Jornaleros.Add(j);
    }
}