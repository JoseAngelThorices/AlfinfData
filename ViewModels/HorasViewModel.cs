using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlfinfData.ViewModels
{
    public partial class HorasViewModel : ObservableObject
    {
        private readonly HorasRepository _horasRepo;
        private readonly CuadrillaRepository _cuadrillaRepo;
        private readonly JornaleroRepository _jornaleroRepo;
        private readonly FichajeRepository _fichajeRepo;

        [ObservableProperty]
        private DateTime fechaSeleccionada = DateTime.Today;

        [ObservableProperty]
        private Cuadrilla? cuadrillaSeleccionada;

        [ObservableProperty]
        private List<JornaleroConHoras> seleccionados = new();

        public ObservableCollection<JornaleroConHoras> JornalerosConHoras { get; } = new();
        private List<JornaleroConHoras> todosLosJornaleros { get; set; } = new();
        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();

        public HorasViewModel(
               HorasRepository horasRepo,
               CuadrillaRepository cuadrillaRepo,
               JornaleroRepository jornaleroRepo,
               FichajeRepository fichajeRepo)
        {
            _horasRepo = horasRepo;
            _cuadrillaRepo = cuadrillaRepo;
            _jornaleroRepo = jornaleroRepo;
            _fichajeRepo = fichajeRepo;
        }

        public async Task CargarCuadrillasAsync()
        {
            var lista = await _cuadrillaRepo.GetAllAsync();

            Cuadrillas.Clear();
            Cuadrillas.Add(new Cuadrilla { IdCuadrilla = 0, Descripcion = "TODOS" }); // opción TODOS
            foreach (var c in lista)
                Cuadrillas.Add(c);

            // Restaurar cuadrilla seleccionada si existe
            var savedId = Preferences.Default.Get("Horas_CuadrillaSeleccionadaId", 0);
            CuadrillaSeleccionada = Cuadrillas.FirstOrDefault(c => c.IdCuadrilla == savedId);
        }

        //Guardar cuadrillas.
        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla? value)
        {
        
            Preferences.Default.Set("Horas_CuadrillaSeleccionadaId", value?.IdCuadrilla ?? 0); // Guardar selección
            FiltrarJornaleros();
        }


        //Funcion filtrar los jornaleros por su cuadrilla
        private void FiltrarJornaleros()
        {
            JornalerosConHoras.Clear();

            var id = CuadrillaSeleccionada?.IdCuadrilla ?? 0;
            var filtrados = id == 0
                ? todosLosJornaleros
                : todosLosJornaleros.Where(j => j.IdCuadrilla == id);

            foreach (var j in filtrados)
                JornalerosConHoras.Add(j);
        }


        public async Task CargarDesdeActivosAsync()
        {
            var hoy = DateTime.Today;
            var activos = await _jornaleroRepo.GetAllAsync();
            var soloActivos = activos.Where(j => j.Activo == true).ToList();

            // Obtenemos todos los fichajes reales de hoy
            var fichajesHoy = await _fichajeRepo.GetJornaleroEntradasAsync();  // Este metodo ya te devuelve los fichajes de tipo "Entrada" de hoy

            todosLosJornaleros.Clear();

            foreach (var j in soloActivos)
            {
                var entrada = fichajesHoy.FirstOrDefault(f => f.IdJornalero == j.IdJornalero);
                if (entrada == null || entrada.HoraEficaz == default)
                    continue;

                var horaInicio = entrada.HoraEficaz;
                var totalHoras = (DateTime.Now - horaInicio).TotalHours;

                var hn = Math.Min(totalHoras, 6.5);
                var he1 = Math.Max(0, totalHoras - 6.5);

                var jHoras = new JornaleroConHoras
                {
                    IdJornalero = j.IdJornalero,
                    Nombre = j.Nombre,
                    IdCuadrilla = j.IdCuadrilla,
                    Hn = Math.Round(hn, 2),
                    He1 = Math.Round(he1, 2),
                    He2 = 0
                };

                todosLosJornaleros.Add(jHoras);

                var existente = await _horasRepo.GetHorasPorJornaleroYFechaAsync(j.IdJornalero, hoy);
                if (existente != null)
                {
                    existente.HN = jHoras.Hn;
                    existente.HE1 = jHoras.He1;
                    existente.HE2 = jHoras.He2;
                    await _horasRepo.ActualizarHorasAsync(existente);
                }
                else
                {
                    var nuevo = new Horas
                    {
                        IdJornalero = j.IdJornalero,
                        Fecha = hoy,
                        HN = jHoras.Hn,
                        HE1 = jHoras.He1,
                        HE2 = jHoras.He2
                    };
                    await _horasRepo.InsertarHorasAsync(nuevo);
                }
            }

            // ✅ Solo una vez, al final
            FiltrarJornaleros();
        }
    }
}

