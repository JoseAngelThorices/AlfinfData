using SQLite;

namespace AlfinfData.Models
{
    public class traza
    {
        [PrimaryKey, AutoIncrement]
        public int IdTraza { get; set; }
        public string Descripcion { get; set; }
        public string Nivel { get; set; }
    }
}
