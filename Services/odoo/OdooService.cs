using System.Net.Http.Json;
using System.Text.Json;
using AlfinfData.Settings;
using Microsoft.Extensions.Options;

namespace AlfinfData.Services.odoo
{
    public class OdooService
    {
        private readonly HttpClient _http; // Cliente HTTP inyectado
        private readonly OdooConfiguracion _cfg; // Configuración con URL, credenciales, etc.
        private bool _isAuthenticated;  // Indica si ya se ha iniciado sesión con Odoo

        public OdooService(HttpClient http, IOptions<OdooConfiguracion> cfg)
        {
            _http = http;
            _cfg = cfg.Value;
        }

        // Método privado para asegurarse de que la sesión está autenticada
        private async Task EnsureAuthenticatedAsync()
        {
            // Si ya está autenticado, salir
            if (_isAuthenticated) return;

            // Payload con estructura JSON para iniciar sesión en Odoo
            var loginPayload = new
            {
                jsonrpc = "2.0",
                method = "call",
                @params = new
                {
                    db = _cfg.Database, 
                    login = _cfg.Username,
                    password = _cfg.Password   
                },
                id = 1
            };

            // Hacemos POST a /web/session/authenticate para loguearnos
            var resp = await _http.PostAsJsonAsync(
                "/web/session/authenticate",
                loginPayload
            );
            resp.EnsureSuccessStatusCode(); // Si hay error de red o credenciales, lanza excepción

            // Si llegó aquí, la cookie de sesión ya fue guardada en el HttpClient
            _isAuthenticated = true;
        }

        // Método público para llamar a cualquier método de Odoo usando JSON-RPC

        public async Task<JsonElement> CallAsync(
            string model,
            string method, 
            object[] args, 
            object? kwargs = null)

        {
            await EnsureAuthenticatedAsync();

            // Construimos el payload con los datos que Odoo espera
            var payload = new
            {
                jsonrpc = "2.0",
                method = "call",
                @params = new
                {
                    model = model,
                    method = method,
                    args = args,
                    kwargs = kwargs ?? new { }
                },
                id = 1
            };

            // ¡OJO! apuntamos al endpoint /web/dataset/call_kw
            var resp = await _http.PostAsJsonAsync("/web/dataset/call_kw", payload);

            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync();// Leemos el contenido de la respuesta como string
            using var doc = JsonDocument.Parse(content);// Parseamos el JSON de respuesta
            if (doc.RootElement.TryGetProperty("error", out var err))
            {
                var msg = err.GetProperty("message").GetString() ?? "Unknown error";
                throw new Exception($"Odoo RPC error: {msg}");
            }
            // Devolvemos solo el contenido del campo "result" (respuesta del método de Odoo)
            return doc.RootElement.GetProperty("result").Clone();
        }

    }

}
