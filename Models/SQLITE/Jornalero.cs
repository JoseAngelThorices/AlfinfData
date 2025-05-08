using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace AlfinfData.Models.SQLITE
{
    public partial class Jornalero : ObservableObject
    {
        [PrimaryKey]
        public int IdJornalero { get; set; }
        public int IdCuadrilla { get; set; }
        public string? Nombre { get; set; }
        public int? NumeroLista { get; set; }

        [ObservableProperty]
        private bool activo;

        public string? TarjetaNFC { get; set; }
    }
}
