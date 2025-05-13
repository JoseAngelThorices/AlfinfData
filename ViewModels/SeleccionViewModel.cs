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

        public SeleccionViewModels(JornaleroRepository repo, CuadrillaRepository repoC)
        {
            _repo = repo;
            _repoC = repoC;
        }

        public async Task CargarCuadrillaAsync()
        {
            var lista = await _repoC.GetAllAsync();

            Cuadrillas.Clear();
            Cuadrillas.Add(new Cuadrilla { IdCuadrilla = 0, Descripcion = "TODOS" });
            foreach (var c in lista)
                Cuadrillas.Add(c);

            // Restaurar cuadrilla guardada
            var savedId = Preferences.Default.Get("CuadrillaSeleccionadaId", 0);
            CuadrillaSeleccionada = Cuadrillas.FirstOrDefault(c => c.IdCuadrilla == savedId);
        }

        public async Task CargarEmpleadosAsync()
        {
            TodosLosJornaleros = await _repo.GetAllAsync();
            FiltrarJornaleros();
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
        }
        public async void SeleccionarTodos()
        {
            foreach (var j in Jornaleros)
                j.Activo = true;

            await _repo.UpdateManyAsync(Jornaleros.ToList());

            // Recargar lista manualmente con nuevos objetos para refrescar UI
            var id = CuadrillaSeleccionada?.IdCuadrilla ?? 0;
            var recargados = await _repo.GetAllAsync();

            TodosLosJornaleros = recargados;

            var filtrados = id == 0
                ? recargados
                : recargados.Where(j => j.IdCuadrilla == id);

            Jornaleros.Clear();
            foreach (var j in filtrados.Select(c => new Jornalero
            {
                IdJornalero = c.IdJornalero,
                IdCuadrilla = c.IdCuadrilla,
                Nombre = c.Nombre,
                NumeroLista = c.NumeroLista,
                Activo = c.Activo,
                TarjetaNFC = c.TarjetaNFC
            }))
            {
                Jornaleros.Add(j);
            }
        }



        public async void QuitarTodos()
        {
            foreach (var j in Jornaleros)
                j.Activo = false;

            await _repo.UpdateManyAsync(Jornaleros.ToList());

            var id = CuadrillaSeleccionada?.IdCuadrilla ?? 0;
            var recargados = await _repo.GetAllAsync();

            TodosLosJornaleros = recargados;

            var filtrados = id == 0
                ? recargados
                : recargados.Where(j => j.IdCuadrilla == id);

            Jornaleros.Clear();
            foreach (var j in filtrados.Select(c => new Jornalero
            {
                IdJornalero = c.IdJornalero,
                IdCuadrilla = c.IdCuadrilla,
                Nombre = c.Nombre,
                NumeroLista = c.NumeroLista,
                Activo = c.Activo,
                TarjetaNFC = c.TarjetaNFC
            }))
            {
                Jornaleros.Add(j);
            }
        }


    }
}
