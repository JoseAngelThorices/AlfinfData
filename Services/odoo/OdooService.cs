using System.Net.Http.Json;
using System.Text.Json;
using AlfinfData.Settings;
using Microsoft.Extensions.Options;

namespace AlfinfData.Services.odoo
{
    public class OdooService
    {
        private readonly HttpClient _http;
        private readonly OdooConfiguracion _cfg;
        private bool _isAuthenticated;

        public OdooService(HttpClient http, IOptions<OdooConfiguracion> cfg)
        {
            _http = http;
            _cfg = cfg.Value;
        }
        private async Task EnsureAuthenticatedAsync()
        {
            if (_isAuthenticated) return;

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

            var resp = await _http.PostAsJsonAsync(
                "/web/session/authenticate",
                loginPayload
            );
            resp.EnsureSuccessStatusCode();

            // Ya se grabó la cookie en el CookieContainer
            _isAuthenticated = true;
        }
       
        public async Task<JsonElement> CallAsync(
    string model,
    string method,
    object[] args,
    object? kwargs = null)
        {
            await EnsureAuthenticatedAsync();

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
            var content = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("error", out var err))
            {
                var msg = err.GetProperty("message").GetString() ?? "Unknown error";
                throw new Exception($"Odoo RPC error: {msg}");
            }
            return doc.RootElement.GetProperty("result").Clone();
        }

    }

}
