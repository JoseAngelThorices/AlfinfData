using SQLite;


namespace AlfinfData.Models.SQLITE
{
    public class Cuadrilla
    {
        [PrimaryKey]
        public int IdCuadrilla { get; set; }
        public string? Descripcion { get; set; }
    }
}
