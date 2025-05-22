using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AlfinfData.ViewModels
{
    public partial class ProduccionViewModel : ObservableObject
    {
        private readonly ProduccionRepository _produccionRepository;
        private readonly CuadrillaRepository _repoC;

        private List<JornaleroConCajas> _seleccionados = new();
        private Cuadrilla? _cuadrillaSeleccionada;

        public ObservableCollection<JornaleroConCajas> JornalerosConCajas { get; } = new();
        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();
        private List<JornaleroConCajas> TodosLosJornaleros { get; set; } = new();

        public ProduccionViewModel(ProduccionRepository produccionRepository, CuadrillaRepository repoC)
        {
            _produccionRepository = produccionRepository;
            _repoC = repoC;
        }

        // Cuadrilla seleccionada desde el Picker
        public Cuadrilla? CuadrillaSeleccionada
        {
            get => _cuadrillaSeleccionada;
            set
            {
                if (_cuadrillaSeleccionada != value)
                {
                    _cuadrillaSeleccionada = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("CuadrillaSeleccionada_Produccion", value?.IdCuadrilla ?? 0);
                    FiltrarJornaleros();
                }
            }
        }

        public void SetSeleccionados(List<JornaleroConCajas> lista) => _seleccionados = lista;

        // Carga las cuadrillas y restaura la última seleccionada
        public async Task CargarCuadrillaAsync()
        {
            var lista = await _repoC.GetAllAsync();
            Cuadrillas.Clear();

            Cuadrillas.Add(new Cuadrilla { IdCuadrilla = 0, Descripcion = "TODOS" });
            foreach (var c in lista)
                Cuadrillas.Add(c);

            var savedId = Preferences.Default.Get("CuadrillaSeleccionada_Produccion", 0);
            CuadrillaSeleccionada = Cuadrillas.FirstOrDefault(c => c.IdCuadrilla == savedId) ?? Cuadrillas.FirstOrDefault();
        }

        // Carga la producción actual de todos los jornaleros activos
        public async Task CargarJornalerosConCajasAsync()
        {
            TodosLosJornaleros = await _produccionRepository.GetJornalerosConCajasActivosAsync();
            FiltrarJornaleros();
        }

        // Filtra jornaleros según cuadrilla seleccionada
        private void FiltrarJornaleros()
        {
            JornalerosConCajas.Clear();
            int id = CuadrillaSeleccionada?.IdCuadrilla ?? 0;

            var filtrados = id == 0
                ? TodosLosJornaleros
                : TodosLosJornaleros.Where(j => j.IdCuadrilla == id);

            foreach (var j in filtrados)
                JornalerosConCajas.Add(j);
        }

        // Inserta un registro de producción para un jornalero
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

        // Aplica la cantidad a todos los seleccionados
        public async Task ProcesarCajasAsync(int cantidad)
        {
            if (_seleccionados.Count == 0)
                return;

            foreach (var j in _seleccionados)
                await InsertProduccionAsync(j.IdJornalero, cantidad);

            await CargarJornalerosConCajasAsync();
        }
    }
}
