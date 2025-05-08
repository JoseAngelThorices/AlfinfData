

namespace AlfinfData.Models.Odoo
{
    public class TarjetaNFC
    {
        public int Id { get; set; }
        public required int IdTarjetaNFC { get; set; }
        public string NumeroSerie { get; set; }
        public Boolean? Activo { get; set; }
        public int? IdJornalero { get; set; }
    }
}
