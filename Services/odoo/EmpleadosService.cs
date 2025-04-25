using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlfinfData.Models.Odoo;

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
            var json = await _odoo.CallAsync(
    model: "hr.employee",
    method: "search_read",
    args: new object[] { },    // vacío
    kwargs: new
    {
        domain = new object[][]
        {
            new object[] { "department_id", "=", 3 }
        },
        fields = new[]
        {
            "id",
            "name",
            "department_id"
        },
        order = "name",
        offset = 0
    }
);

            // 2) Mapear el JsonElement (que es un array) a List<Employee>
            var list = new List<Empleado>();
            foreach (var item in json.EnumerateArray())
            {
                var deptElemento = item.GetProperty("department_id");
                int deptId = 0;
                if (deptElemento.ValueKind == JsonValueKind.Array && deptElemento.GetArrayLength() >= 1)
                {
                    deptId = deptElemento[0].GetInt32();
                }
                else if (deptElemento.ValueKind == JsonValueKind.Number)
                {
                    deptId = deptElemento.GetInt32();
                }
                list.Add(new Empleado
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Nombre = item.GetProperty("name").GetString()!,
                    Id_Departamento = deptId
                });
            }

            // 3) Devolver la lista
            return list;
        }


    }
}
