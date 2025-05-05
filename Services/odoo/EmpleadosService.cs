using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlfinfData.Models.Odoo;

namespace AlfinfData.Services.odoo
{
    // Interfaz del servicio de empleados
    public interface IEmpleadosService
    {
        Task<IEnumerable<Empleado>> GetAllAsync(); // Método para obtener todos los empleados
    }

    // Implementación concreta del servicio de empleados
    public class EmpleadosService : IEmpleadosService
    {
        private readonly OdooService _odoo; // Inyectamos el servicio genérico para llamar a Odoo

        public EmpleadosService(OdooService odoo)
            => _odoo = odoo;

        // Método principal que obtiene empleados desde Odoo usando search_read
        public async Task<IEnumerable<Empleado>> GetAllAsync()
        {
            // Llamamos a Odoo usando el modelo 'hr.employee' y método 'search_read'
            var json = await _odoo.CallAsync(
                model: "hr.employee",
                method: "search_read",
                args: new object[] { }, 
                kwargs: new
                {
                    domain = new object[][]
                    {
                        new object[] { "department_id", "in", new object[] { 3, 4 } } // Solo empleados de los departamentos 3 y 4
                    },
                    fields = new[]
                    {
                        "id", "name", "department_id", "active" // Campos que queremos traer
                    },
                    order = "name",  
                    offset = 0   
                }
            );

            // Convertimos manualmente el JsonElement en una lista de objetos Empleado
            var list = new List<Empleado>();
            foreach (var item in json.EnumerateArray())
            {
                // Obtener el ID del departamento 
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

                // Crear objeto Empleado con los datos que vienen de Odoo
                list.Add(new Empleado
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Nombre = item.GetProperty("name").GetString()!,
                    Id_Departamento = deptId
                });
            }

            // 3) Devolver la lista final de empleados
            return list;
        }
    }
}
