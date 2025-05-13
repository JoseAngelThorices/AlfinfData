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

        [ObservableProperty]
        private Cuadrilla? _cuadrillaSeleccionada;

        [ObservableProperty]
        private int seleccionados;

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

        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla? value)
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

            ActualizarContador();
        }

        public async void SeleccionarTodos()
        {
            foreach (var j in Jornaleros)
                j.Activo = true;

            await _repo.UpdateManyAsync(Jornaleros);

            // Recargar desde la BD los datos actualizados
            await RecargarDesdeBD();
        }

        public async void QuitarTodos()
        {
            foreach (var j in Jornaleros)
                j.Activo = false;

            await _repo.UpdateManyAsync(Jornaleros);

            await RecargarDesdeBD();
        }


        public async Task ActualizarJornaleroAsync(Jornalero j)
        {
            await _repo.UpdateAsync(j);
        }

        public void ActualizarContador()
        {
            Seleccionados = Jornaleros.Count(j => j.Activo == true);
        }


        private async Task RecargarDesdeBD()
        {
            var cuadrillaId = CuadrillaSeleccionada?.IdCuadrilla ?? 0;

            var actualizados = await _repo.GetAllAsync();
            TodosLosJornaleros = actualizados;

            var filtrados = CuadrillaSeleccionada == null
                ? actualizados
                : actualizados.Where(j => j.IdCuadrilla == cuadrillaId);

            Jornaleros.Clear();
            foreach (var j in filtrados)
                Jornaleros.Add(j);

            ActualizarContador();
        }

    }
}
