using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using AlfinfData.Models;
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
        //public async Task<string?> TryLoginAsync()
        //{
        //    try
        //    {
        //        await LoginAsync();
        //        return null;    // OK
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        // Incluye código de estado si lo hay
        //        return $"HTTP {(int?)httpEx.StatusCode}: {httpEx.Message}";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
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
                    model = model,      // p.ej. "hr.employee"
                    method = method,     // p.ej. "search_read"
                    args = args,       // aquí pones [] si no hay args posicionales
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

        //public async Task<JsonElement> CallAsync(string service,string rpcMethod,object[] positionalArgs,object? kwargs = null)
        //{
        //    await LoginAsync();

        //        // Construimos la lista: db, uid, password, modelo, método, parámetros
        //        var allArgs = new List<object>
        //    {
        //        _cfg.Database,
        //        _uid,
        //        _cfg.Password
        //    };
        //        allArgs.AddRange(positionalArgs);

        //    var payload = new
        //    {
        //        jsonrpc = "2.0",
        //        method = "call",    // SIEMPRE "call"
        //        @params = new
        //        {
        //            service = service,     // "object"
        //            method = rpcMethod,   // "execute_kw"
        //            args = allArgs.ToArray(),
        //            kwargs = kwargs ?? new { }
        //        },
        //        id = 1
        //    };

        //    var resp = await _http.PostAsJsonAsync("/jsonrpc", payload);
        //    //var json = await resp.Content.ReadAsStringAsync();

        //    // Manda el JSON al Output de Visual Studio
        //    //System.Diagnostics.Debug.WriteLine(json);
        //    var jsonPayload = JsonSerializer.Serialize(payload);
        //    Debug.WriteLine("REQUEST PAYLOAD:\n" + jsonPayload);
        //    var respText = await resp.Content.ReadAsStringAsync();
        //    Debug.WriteLine("RESPONSE TEXT:\n" + respText);
        //    resp.EnsureSuccessStatusCode();

        //    var content = await resp.Content.ReadAsStringAsync();
        //    using var doc = JsonDocument.Parse(content);
        //    var root = doc.RootElement;

        //    if (root.TryGetProperty("error", out var err))
        //    {
        //        var msg = err.GetProperty("message").GetString() ?? "Unknown error";
        //        throw new Exception($"Odoo RPC error: {msg}");
        //    }

        //    var resultClone = root.GetProperty("result").Clone();
        //    return resultClone;
        //}

    }

}
