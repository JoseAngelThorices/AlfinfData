using SQLite;
using System;

namespace AlfinfData.Models.SQLITE
{
    public class Fichaje
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int? IdJornalero { get; set; }
        public string TipoFichaje { get; set; } // Entrada o Salida
        public DateTime? InstanteFichaje { get; set; }
        public DateTime HoraEficaz { get; set; } // Guardará la hora que introduzcamos con la función de "nuevo dia" y se podrá cambiar con el boton de hora de la ventana "Entrada"
    }
}
