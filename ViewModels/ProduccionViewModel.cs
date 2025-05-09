using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlfinfData.ViewModels
{
    public partial class ProduccionViewModel : ObservableObject
    {
        private readonly ProduccionRepository _produccionRepository;
        private readonly CuadrillaRepository _repoC;

        [ObservableProperty]
        private List<JornaleroConCajas> seleccionados = new();

        public ObservableCollection<JornaleroConCajas> JornalerosConCajas { get; set; } = new();
        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();

        private List<JornaleroConCajas> TodosLosJornaleros { get; set; } = new();

        [ObservableProperty]
        private Cuadrilla cuadrillaSeleccionada;

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

        public async Task CargarJornalerosConCajasAsync()
        {
            var jornalerosConCajas = await _produccionRepository.GetJornalerosConCajasAsync();

            TodosLosJornaleros = jornalerosConCajas.ToList();

            JornalerosConCajas.Clear();
            JornalerosConCajas = new ObservableCollection<JornaleroConCajas>(jornalerosConCajas);
            OnPropertyChanged(nameof(JornalerosConCajas));  // Notifica al UI que cambió la lista

        }

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

        // Método para añadir o restar cajas
        public async Task ProcesarCajasAsync(int cantidad)
        {
            if (Seleccionados is null || Seleccionados.Count == 0)
                return;

            foreach (var j in Seleccionados)
            {
                await _produccionRepository.InsertProduccionAsync(j.IdJornalero, cantidad);
            }

            await CargarJornalerosConCajasAsync(); // vuelve a calcular TotalCajas
            FiltrarJornaleros();                   // mantiene el filtro si hay
        }

    }
}
