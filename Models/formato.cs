using SQLite;

namespace AlfinfData.Models
{
    public class formato
    {
        [PrimaryKey, AutoIncrement]
        public int IdFormato { get; set; }
        public string Descripcion { get; set; }
    }
}
