using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlfinfData.ViewModels
{
    public partial  class ProduccionViewModel : ObservableObject
    {
        private readonly ProduccionRepository _produccionRepository;
        private readonly CuadrillaRepository _repoC;

        public ObservableCollection<JornaleroConCajas> JornalerosConCajas { get; set; } = new();
        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();
        private List<JornaleroConCajas> TodosLosJornaleros { get; set; } = new();


        public ProduccionViewModel(ProduccionRepository produccionRepository, CuadrillaRepository repoC)
        {
            _produccionRepository = produccionRepository;
            _repoC = repoC;
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
            JornalerosConCajas.Clear();

            var cuadrillaId = CuadrillaSeleccionada?.IdCuadrilla ?? 0;

            var listaFiltrada = CuadrillaSeleccionada == null
                ? TodosLosJornaleros
                : TodosLosJornaleros.Where(j => j.IdCuadrilla == cuadrillaId);

            foreach (var j in listaFiltrada)
                JornalerosConCajas.Add(j);
        }

        public async Task CargarJornalerosConCajasAsync()
        {
            var jornalerosConCajas = await _produccionRepository.GetJornalerosConCajasAsync();

            JornalerosConCajas.Clear();
            foreach (var jornalero in jornalerosConCajas)
            {
                JornalerosConCajas.Add(jornalero);
            }
        }
    }
}
