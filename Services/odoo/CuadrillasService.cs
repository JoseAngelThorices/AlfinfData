using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlfinfData.Models.Odoo;

namespace AlfinfData.Services.odoo
{
    public interface ICuadrillasService
    {
        Task<IEnumerable<CuadrillaOdoo>> GetAllAsync();
    }
    public class CuadrillaService : ICuadrillasService
    {
        private readonly OdooService _odoo;

        public CuadrillaService(OdooService odoo)
            => _odoo = odoo;

        public async Task<IEnumerable<CuadrillaOdoo>> GetAllAsync()
        {
            var json = await _odoo.CallAsync(
            model: "hr.department",
            method: "search_read",
            args: new object[] { },    // vacío
            kwargs: new
            {
                domain = new object[][]
            {
                new object[] { "id", "in", new object[] { 3, 4 } }
            },
                fields = new[]
            {
                "id",
                "name",
            },
                order = "name",
                offset = 0
            });

            // 2) Mapear el JsonElement (que es un array) a List<Cuadrilla>
            var list = new List<CuadrillaOdoo>();
            foreach (var item in json.EnumerateArray())
            {
                list.Add(new CuadrillaOdoo
                {
                    IdCuadrilla = item.GetProperty("id").GetInt32(),
                    Descripcion = item.GetProperty("name").GetString()!,
                });
            }

            // 3) Devolver la lista
            return list;
        }


    }
}
