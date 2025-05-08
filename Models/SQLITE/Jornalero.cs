using SQLite;

namespace AlfinfData.Models.SQLITE
{
    public partial class Jornalero
    {
        [PrimaryKey]
        public int IdJornalero { get; set; }
        public int IdCuadrilla { get; set; }
        public string? Nombre { get; set; }
        public int? NumeroLista { get; set; }
        public bool Activo { get; set; }
        public string? TarjetaNFC { get; set; }
    }
}
