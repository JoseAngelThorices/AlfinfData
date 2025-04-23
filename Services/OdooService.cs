using System.Net.Http.Json;
using System.Text.Json;
using AlfinfData.Models;

namespace AlfinfData.Services
{
    public class OdooService
    {
        private readonly HttpClient _http;
        private readonly string _db = "postgres";
        private readonly string _user = "josedbourre@gmail.com";
        private readonly string _password = "odoo";
        private int _uid;

        public OdooService(HttpClient http) => _http = http;

        // 1) Autenticación: guarda el UID
        public async Task LoginAsync()
        {
            if (_uid != 0) return; // ya autenticado

            var payload = new
            {
                jsonrpc = "2.0",
                method = "call",
                @params = new
                {
                    service = "common",
                    method = "login",
                    args = new object[] { _db, _user, _password }
                },
                id = 1
            };

            var resp = await _http.PostAsJsonAsync("/jsonrpc", payload);
            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            _uid = doc.RootElement.GetProperty("result").GetInt32();
            if (_uid == 0)
                throw new Exception("Odoo: credenciales inválidas");
        }

        // 2) Lectura de empleados
        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            await LoginAsync();

            var payload = new
            {
                jsonrpc = "2.0",
                method = "call",
                @params = new
                {
                    service = "object",
                    method = "execute_kw",
                    args = new object[] {
                    _db,
                    _uid,
                    _password,
                    "hr.employee",     // modelo
                    "search_read",     // método
                    new object[] {
                        new object[] { } // dominio vacío → todos
                    },
                    new {
                        fields = new[] {
                            "id", "name", "work_email", "job_id", "department_id"
                        },
                        limit = 100         // límite de registros
                    }
                }
                },
                id = 2
            };

            var resp = await _http.PostAsJsonAsync("/jsonrpc", payload);
            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var array = doc.RootElement.GetProperty("result").EnumerateArray();

            var lista = new List<Empleado>();
            foreach (var item in array)
            {
                lista.Add(new Empleado
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Nombre = item.GetProperty("name").GetString()!,
                    EmailTrabajo = item.GetProperty("work_email").GetString(),
                    Puesto = item.GetProperty("job_id")[1].GetString()!,
                    Departamento = item.GetProperty("department_id")[1].GetString()!
                });
            }
            return lista;
        }

    }
}
