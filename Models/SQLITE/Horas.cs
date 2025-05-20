using SQLite;
using System;

namespace AlfinfData.Models.SQLITE
{
    public class Horas
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
