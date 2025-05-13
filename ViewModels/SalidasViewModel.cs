using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AlfinfData.ViewModels
{
    public partial class SalidasViewModel : ObservableObject
    {
        private readonly JornaleroRepository _jornaleroRepo;
        private readonly CuadrillaRepository _cuadrillaRepo;

        [ObservableProperty]
        private Cuadrilla cuadrillaSeleccionada;

        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();
        public ObservableCollection<Jornalero> JornalerosPendientes { get; } = new();

        public SalidasViewModel(JornaleroRepository jornaleroRepo, CuadrillaRepository cuadrillaRepo)
        {
            _jornaleroRepo = jornaleroRepo;
            _cuadrillaRepo = cuadrillaRepo;
        }

        public async Task CargarCuadrillasAsync()
        {
            Cuadrillas.Clear();
            var lista = await _cuadrillaRepo.GetAllAsync();
            foreach (var c in lista)
                Cuadrillas.Add(c);
        }

        public async Task CargarJornalerosPendientesAsync()
        {
            JornalerosPendientes.Clear();

            if (CuadrillaSeleccionada == null)
                return;

            // Sin filtrar por activos
            var lista = await _jornaleroRepo.GetJornalerosActivosPorCuadrillaAsync(CuadrillaSeleccionada.IdCuadrilla);

            foreach (var j in lista)
                JornalerosPendientes.Add(j);
        }


        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla value)
        {
            // Refresca la lista cuando cambia la cuadrilla
            _ = CargarJornalerosPendientesAsync();
        }
    }
}
