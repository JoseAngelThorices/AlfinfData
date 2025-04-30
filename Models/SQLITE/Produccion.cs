using SQLite;
using System;

namespace AlfinfData.Models.SQLITE
{
    public class Produccion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int IdJornalero { get; set; }
        [Indexed]
        public int? IdTraza { get; set; }
        [Indexed]
        public int? IdFormato { get; set; }
        public DateTime Timestamp { get; set; }
        public int Cajas { get; set; }
    }
}
