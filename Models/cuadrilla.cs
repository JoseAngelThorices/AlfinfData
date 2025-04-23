using SQLite;

namespace AlfinfData.Models
{
    public class cuadrilla
    {
        [PrimaryKey, AutoIncrement]
        public int IdCuadrilla { get; set; }
        public string Descripcion { get; set; }
    }
}
