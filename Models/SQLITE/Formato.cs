using SQLite;

namespace AlfinfData.Models.SQLITE
{
    public class Formato
    {
        [PrimaryKey, AutoIncrement]
        public int IdFormato { get; set; }
        public string Descripcion { get; set; }
    }
}
