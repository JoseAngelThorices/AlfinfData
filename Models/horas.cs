using SQLite;
using System;

namespace AlfinfData.Models
{
    public class horas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int IdJornalero { get; set; }
        public DateTime Fecha { get; set; }
        public double HN { get; set; }
        public double HE1 { get; set; }
        public double HE2 { get; set; }
    }
}
