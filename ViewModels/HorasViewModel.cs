using AlfinfData.Models.SQLITE;
using AlfinfData.Services.BdLocal;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlfinfData.ViewModels
{
    public partial class HorasViewModel : ObservableObject
    {
        private readonly HorasRepository _horasRepo;
        private readonly CuadrillaRepository _cuadrillaRepo;
        private readonly FichajeRepository _fichajeRepo;

        [ObservableProperty]
        private DateTime fechaSeleccionada = DateTime.Today;

        [ObservableProperty]
        private Cuadrilla cuadrillaSeleccionada;

        [ObservableProperty]
        private List<JornaleroConHoras> seleccionados = new();

        public ObservableCollection<JornaleroConHoras> JornalerosConHoras { get; set; } = new();
        private List<JornaleroConHoras> todosLosJornaleros { get; set; } = new();

        public ObservableCollection<Cuadrilla> Cuadrillas { get; } = new();

        public HorasViewModel(HorasRepository horasRepo, CuadrillaRepository cuadrillaRepo, FichajeRepository fichajeRepo)
        {
            _horasRepo = horasRepo;
            _cuadrillaRepo = cuadrillaRepo;
            _fichajeRepo = fichajeRepo;
        }

        public async Task CargarCuadrillasAsync()
        {
            var lista = await _cuadrillaRepo.GetAllAsync();
            Cuadrillas.Clear();
            foreach (var c in lista)
                Cuadrillas.Add(c);
        }

        public async Task GuardarHorasAsync()
        {
            if (JornalerosConHoras == null || !JornalerosConHoras.Any())
                return;

            foreach (var j in JornalerosConHoras)
            {
                var horas = new Horas
                {
                    IdJornalero = j.IdJornalero,
                    Fecha = FechaSeleccionada, // este campo ya lo tienes
                    HN = j.Hn,
                    HE1 = j.He1,
                    HE2 = j.He2
                };

                await _horasRepo.InsertarHorasAsync(horas);
            }
        }




        public async Task CargarJornalerosConHorasAsync()
        {
            var lista = await _horasRepo.GetJornalerosConHorasAsync(FechaSeleccionada);
            todosLosJornaleros = lista;
            FiltrarJornaleros();
        }

        partial void OnCuadrillaSeleccionadaChanged(Cuadrilla value)
        {
            FiltrarJornaleros();
        }

        private void FiltrarJornaleros()
        {
            JornalerosConHoras.Clear();

            var idCuadrilla = CuadrillaSeleccionada?.IdCuadrilla ?? 0;
            var filtrados = CuadrillaSeleccionada == null
                ? todosLosJornaleros
                : todosLosJornaleros.Where(j => j.IdCuadrilla == idCuadrilla);

            foreach (var j in filtrados)
                JornalerosConHoras.Add(j);
        }


        public async Task CargarJornalerosDesdeFichajesAsync()
        {
            var fichajes = await _fichajeRepo.GetAllAsync();

            var fichajesDelDia = fichajes
                .Where(f => f.InstanteFichaje.HasValue &&
                            f.IdJornalero.HasValue &&
                            f.InstanteFichaje.Value.Date == FechaSeleccionada.Date)
                .OrderBy(f => f.InstanteFichaje)
                .GroupBy(f => f.IdJornalero.Value);

            todosLosJornaleros.Clear();

            foreach (var grupo in fichajesDelDia)
            {
                double hn = 0;
                double he1 = 0;
                var lista = grupo.ToList();

                for (int i = 0; i < lista.Count - 1; i += 2)
                {
                    var entrada = lista[i];
                    var salida = lista[i + 1];

                    if (entrada.TipoFichaje != "Entrada" || salida.TipoFichaje != "Salida")
                        continue;

                    if (!entrada.InstanteFichaje.HasValue || !salida.InstanteFichaje.HasValue)
                        continue;

                    var horas = (salida.InstanteFichaje.Value - entrada.InstanteFichaje.Value).TotalHours;

                    if (horas <= 6.5)
                        hn += horas;
                    else
                    {
                        hn += 6.5;
                        he1 += horas - 6.5;
                    }
                }

                // Si hay una entrada sin su salida correspondiente
                if (lista.Count % 2 != 0 && lista.Last().TipoFichaje == "Entrada" && lista.Last().InstanteFichaje.HasValue)
                {
                    var horas = (DateTime.Now - lista.Last().InstanteFichaje.Value).TotalHours;

                    if (horas <= 6.5)
                        hn += horas;
                    else
                    {
                        hn += 6.5;
                        he1 += horas - 6.5;
                    }
                }

                todosLosJornaleros.Add(new JornaleroConHoras
                {
                    IdJornalero = grupo.Key,
                    Hn = Math.Round(hn, 2),
                    He1 = Math.Round(he1, 2),
                    He2 = 0,
                    IdCuadrilla = 0, // Puedes mejorarlo si tienes el dato
                    Nombre = $"Jornalero {grupo.Key}" // Puedes buscarlo si tienes lista de jornaleros
                });
            }

            FiltrarJornaleros();
        }





    }
}
