using SQLite;
using System;

namespace AlfinfData.Models.SQLITE
{
    public class Fichaje
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int IdJornalero { get; set; }
        public string TipoFichaje { get; set; } // Entrada o Salida
        public DateTime InstanteFichaje { get; set; }
        public double HoraEficaz { get; set; }
    }
}
