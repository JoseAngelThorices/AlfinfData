using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Storage;

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
        private Cuadrilla? cuadrillaSeleccionada;

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
            Cuadrillas.Add(new Cuadrilla { IdCuadrilla = 0, Descripcion = "TODOS" });
            foreach (var c in lista)
                Cuadrillas.Add(c);

            var savedId = Preferences.Default.Get("CuadrillaSeleccionadaId", 0);
            CuadrillaSeleccionada = Cuadrillas.FirstOrDefault(c => c.IdCuadrilla == savedId);
        }

        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla? value)
        {
            Preferences.Default.Set("CuadrillaSeleccionadaId", value?.IdCuadrilla ?? 0);
            FiltrarJornaleros();
        }

        private void FiltrarJornaleros()
        {
            Jornaleros.Clear();

            var id = CuadrillaSeleccionada?.IdCuadrilla ?? 0;
            var filtrados = id == 0
                ? TodosLosJornaleros
                : TodosLosJornaleros.Where(j => j.IdCuadrilla == id);

            foreach (var j in filtrados)
                Jornaleros.Add(j);

            ActualizarContador();
        }

        public async void SeleccionarTodos()
        {
            foreach (var j in Jornaleros)
                j.Activo = true;

            await _repo.UpdateManyAsync(Jornaleros.ToList());
            await RecargarDesdeBD();
        }

        public async void QuitarTodos()
        {
            foreach (var j in Jornaleros)
                j.Activo = false;

            await _repo.UpdateManyAsync(Jornaleros.ToList());
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
            var actualizados = await _repo.GetAllAsync();
            TodosLosJornaleros = actualizados;

            FiltrarJornaleros();
        }
    }
}
