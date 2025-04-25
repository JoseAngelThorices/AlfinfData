
namespace AlfinfData.Models.Odoo
{
    public class Empleado
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public int Id_Departamento { get; set; }
    }
}
