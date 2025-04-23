using SQLite;

namespace AlfinfData.Models
{
    public class jornalero
    {
        [PrimaryKey, AutoIncrement]
        public int IdJornalero { get; set; }


        [Indexed]
        public int IdCuadrilla { get; set; }
        public string Nombre { get; set; }
        public int NumeroLista { get; set; }
        public string TarjetaNFC { get; set; }
    }
}