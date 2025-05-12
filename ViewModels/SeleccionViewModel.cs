using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlfinfData.ViewModels
{
    public partial class SeleccionViewModels : ObservableObject
    {
        private readonly JornaleroRepository _repo;
        private readonly CuadrillaRepository _repoC;

        public ObservableCollection<Jornalero> Jornaleros { get; } = new();
        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();
        private List<Jornalero> TodosLosJornaleros { get; set; } = new();

        public SeleccionViewModels(JornaleroRepository repo, CuadrillaRepository repoC)
        {
            _repo = repo;
            _repoC = repoC;
        }

        public async Task CargarEmpleadosAsync()
        {
            TodosLosJornaleros = await _repo.GetAllAsync();
            FiltrarJornaleros();
        }

        public async Task CargarCuadrillaAsync()
        {
            var lista = await _repoC.GetAllAsync();
            Cuadrillas.Clear();
            foreach (var c in lista)
                Cuadrillas.Add(c);
        }

        [ObservableProperty]
        private Cuadrilla cuadrillaSeleccionada;

        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla value)
        {
            FiltrarJornaleros();
        }

        private void FiltrarJornaleros()
        {
            Jornaleros.Clear();
            var cuadrillaId = CuadrillaSeleccionada?.IdCuadrilla ?? 0;

            var listaFiltrada = CuadrillaSeleccionada == null
                ? TodosLosJornaleros
                : TodosLosJornaleros.Where(j => j.IdCuadrilla == cuadrillaId);

            foreach (var j in listaFiltrada)
                Jornaleros.Add(j);
        }

        public void SeleccionarTodos()
        {
            foreach (var jornalero in Jornaleros)
                jornalero.Activo = true;
        }

        public void QuitarTodos()
        {
            foreach (var jornalero in Jornaleros)
                jornalero.Activo = false;
        }
    }
}
