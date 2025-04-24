
namespace AlfinfData.Models.Odoo
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? EmailTrabajo { get; set; }
        public string Puesto { get; set; } = "";
        public string Departamento { get; set; } = "";
    }
}
