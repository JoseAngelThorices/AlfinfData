using SQLite;

namespace AlfinfData.Models.SQLITE
{
    public class Traza
    {
        [PrimaryKey, AutoIncrement]
        public int IdTraza { get; set; }
        public string Descripcion { get; set; }
        public string Nivel { get; set; }
    }
}
