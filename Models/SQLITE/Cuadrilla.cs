using SQLite;


namespace AlfinfData.Models.SQLITE
{
    public class Cuadrilla
    {
        [PrimaryKey, AutoIncrement]
        public int IdCuadrilla { get; set; }
        public int idOdoo { get; set; }
        public string? Descripcion { get; set; }
    }
}
