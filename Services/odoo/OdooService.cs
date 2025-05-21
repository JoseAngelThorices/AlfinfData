using System.Net.Http.Json;
using System.Text.Json;

namespace AlfinfData.Services.odoo
{
    public class OdooService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfigService _config;
        private bool _isAuthenticated;

        public OdooService(IHttpClientFactory factory, IConfigService config)
        {
            _factory = factory;
            _config = config;
            _config.ConfigChanged += (_, __) => _isAuthenticated = false;
        }

        private HttpClient Client => _factory.CreateClient("Odoo");


        // Método privado para asegurarse de que la sesión está autenticada
        private async Task EnsureAuthenticatedAsync()
        {
            // 1) Si ya está autenticado, salir
            if (_isAuthenticated)
                return;

            // 2) Crea el HttpClient configurado
            var http = Client;

            // 3) Lee las credenciales cifradas
            var (user, pass, nameDateBase) = await _config.GetCredentialsAsync();

            // 4) Construye el payload usando user/pass
            var loginPayload = new
            {
                jsonrpc = "2.0",
                method = "call",
                @params = new
                {
                    db = nameDateBase, // o donde guardes el nombre de la base
                    login = user,
                    password = pass
                },
                id = 1
            };

            // 5) Lanza la petición de autenticación
            var resp = await http.PostAsJsonAsync("/web/session/authenticate", loginPayload);
            resp.EnsureSuccessStatusCode();

            // 6) Marca como autenticado
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
            var http = Client;
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
            var resp = await http.PostAsJsonAsync("/web/dataset/call_kw", payload);

            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync();// Leemos el contenido de la respuesta como string
            using var doc = JsonDocument.Parse(content);// Parseamos el JSON de respuesta
            if (doc.RootElement.TryGetProperty("error", out var err))
            {
                var data = err.GetProperty("data");
                var detalle = data.GetProperty("message").GetString();
                var debug = data.GetProperty("debug").GetString();
                throw new Exception($"Odoo RPC error: {detalle}\nDEBUG: {debug}");
            }
            // Devolvemos solo el contenido del campo "result" (respuesta del método de Odoo)
            return doc.RootElement.GetProperty("result").Clone();
        }

    }

}
