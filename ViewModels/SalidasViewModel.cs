using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;

namespace AlfinfData.ViewModels
{
    public partial class SalidasViewModel : ObservableObject
    {
        private readonly JornaleroRepository _jornaleroRepo;
        private readonly CuadrillaRepository _cuadrillaRepo;

        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();
        public ObservableCollection<Jornalero> JornalerosPendientes { get; } = new();
        private List<Jornalero> TodosLosJornaleros { get; set; } = new();

        [ObservableProperty]
        private Cuadrilla? cuadrillaSeleccionada;

        public SalidasViewModel(JornaleroRepository jornaleroRepo, CuadrillaRepository cuadrillaRepo)
        {
            _jornaleroRepo = jornaleroRepo;
            _cuadrillaRepo = cuadrillaRepo;

            // Restaurar cuadrilla guardada
            var idGuardado = Preferences.Default.Get("CuadrillaSeleccionada_Salidas", 0);
            CuadrillaSeleccionada = Cuadrillas.FirstOrDefault(c => c.IdCuadrilla == idGuardado);
        }

        public async Task CargarCuadrillasAsync()
        {
            Cuadrillas.Clear();
            Cuadrillas.Add(new Cuadrilla { IdCuadrilla = 0, Descripcion = "TODOS" }); // Agregar opción TODOS

            var lista = await _cuadrillaRepo.GetAllAsync();
            foreach (var c in lista)
                Cuadrillas.Add(c);

            // Restaurar la selección guardada
            var idGuardado = Preferences.Default.Get("CuadrillaSeleccionada_Salidas", 0);
            CuadrillaSeleccionada = Cuadrillas.FirstOrDefault(c => c.IdCuadrilla == idGuardado);
        }

        public async Task CargarJornalerosPendientesAsync()
        {
            JornalerosPendientes.Clear();

            var todos = await _jornaleroRepo.GetAllAsync();
            TodosLosJornaleros = todos.Where(j => j.Activo == true).ToList();

            IEnumerable<Jornalero> filtrados;

            if (CuadrillaSeleccionada == null || CuadrillaSeleccionada.IdCuadrilla == 0)
                filtrados = TodosLosJornaleros;
            else
                filtrados = TodosLosJornaleros.Where(j => j.IdCuadrilla == CuadrillaSeleccionada.IdCuadrilla);

            foreach (var j in filtrados)
                JornalerosPendientes.Add(j);
        }

        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla? value)
        {
            Preferences.Default.Set("CuadrillaSeleccionada_Salidas", value?.IdCuadrilla ?? 0);
            _ = CargarJornalerosPendientesAsync();
        }
    }
}
