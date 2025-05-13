
namespace AlfinfData.Models.SQLITE
{
    public class JornaleroEntrada
    {
        public int IdJornalero { get; set; }
        public string Nombre { get; set; }
        public DateTime HoraEficaz { get; set; }
        public string HoraFormateada => HoraEficaz.ToString("HH:mm");

    }
}
