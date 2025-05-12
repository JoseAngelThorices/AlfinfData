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
            OnPropertyChanged(nameof(JornalerosConCajas));
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

        // Método para añadir un registro de producción
        public async Task InsertProduccionAsync(int idJornalero, int cajas)
        {
            var produccion = new Produccion
            {
                IdJornalero = idJornalero,
                Cajas = cajas,
                Timestamp = DateTime.Now
            };

            await _produccionRepository.InsertProduccionAsync(produccion);
        }

        // Método para añadir o restar cajas a los seleccionados
        public async Task ProcesarCajasAsync(int cantidad)
        {
            if (Seleccionados is null || Seleccionados.Count == 0)
                return;

            foreach (var j in Seleccionados)
            {
                await InsertProduccionAsync(j.IdJornalero, cantidad);
            }

            await CargarJornalerosConCajasAsync();
            FiltrarJornaleros();
        }
    }
}
