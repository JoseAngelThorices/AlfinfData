using SQLite;

namespace AlfinfData.Models.SQLITE
{
    public class Jornalero
    {
        [PrimaryKey, AutoIncrement]
        public int IdJornalero { get; set; }
        public int IdCuadrilla { get; set; }
        [Indexed(Name = "Id_Jornalero_IdOdoo", Unique = true)]
        public int? IdOdoo { get; set; }          
        public string? Nombre { get; set; }
        public int? NumeroLista { get; set; }
        public Boolean TarjetaNFC { get; set; }
    }
}