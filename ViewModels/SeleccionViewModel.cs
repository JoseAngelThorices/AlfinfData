using System.Collections.ObjectModel;
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

#pragma warning disable MVWMTK0045
        [ObservableProperty]
        private Cuadrilla? cuadrillaSeleccionada;
#pragma warning restore MVWMTK0045

#pragma warning disable MVWMTK0045
        [ObservableProperty]
        private int seleccionados;
#pragma warning disable MVWMTK0045
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
            ActualizarContador();
        }

        

        public void ActualizarContador()
        {
            if (Jornaleros != null)
                Seleccionados = Jornaleros.Count(j => j.Activo == true);
            else
                Seleccionados = 0;
        }

        public async Task ActualizarJornaleroAsync(Jornalero j)
        {
            await _repo.SetActiveAsync(j.IdJornalero, j.Activo == true);
        }


        public async Task SeleccionarTodos() => await AplicarCambioYRefrescar(true);
        public async Task QuitarTodos() => await AplicarCambioYRefrescar(false);

        private async Task AplicarCambioYRefrescar(bool activar)
        {
            if (CuadrillaSeleccionada == null)
                return;

            foreach (var j in Jornaleros)
                j.Activo = activar;

            await _repo.UpdateManyAsync(Jornaleros.ToList());

            var id = CuadrillaSeleccionada.IdCuadrilla;
            TodosLosJornaleros = await _repo.GetAllAsync();

            var filtrados = id == 0
                ? TodosLosJornaleros
                : TodosLosJornaleros.Where(j => j.IdCuadrilla == id);

            Jornaleros.Clear();
            foreach (var j in filtrados)
                Jornaleros.Add(j);

            ActualizarContador();
        }



    }
}
