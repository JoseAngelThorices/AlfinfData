using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlfinfData.Models;

namespace AlfinfData.Services.odoo
{
    public interface IEmpleadosService
    {
        Task<IEnumerable<Empleado>> GetAllAsync();
    }

    public class EmpleadosService : IEmpleadosService
    {
        private readonly OdooService _odoo;

        public EmpleadosService(OdooService odoo)
            => _odoo = odoo;

        public async Task<IEnumerable<Empleado>> GetAllAsync()
        {
            // 1) Llamada genérica al RPC de Odoo
            var json = await _odoo.CallAsync(
                service: "object",
                rpcMethod: "execute_kw",
                positionalArgs: new object[]{
                    "hr.employee", // el modelo
                    "search_read", // el método
                    new object[] { new object[] { } }},
                           kwargs: new { fields = new[] { "id", "name" } });

            // 2) Mapear el JsonElement (que es un array) a List<Employee>
            var list = new List<Empleado>();
            foreach (var item in json.EnumerateArray())
            {
                list.Add(new Empleado
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Nombre = item.GetProperty("name").GetString()!
                });
            }

            // 3) Devolver la lista
            return list;
        }


    }
}
