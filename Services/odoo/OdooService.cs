using System;
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
        private int _uid;
        private bool _isLogged;

        public OdooService(HttpClient http, IOptions<OdooConfiguracion> cfg)
        {
            _http = http;
            _cfg = cfg.Value;
        }
        private async Task LoginAsync()
        {
            // Si ya hicimos login, no volvemos a llamarlo
            if (_isLogged) return;

            var payload = new
            {
                jsonrpc = "2.0",
                method = "call",
                @params = new
                {
                    service = "common",
                    method = "login",
                    args = new object[]
                    {
                    _cfg.Database,
                    _cfg.Username,
                    _cfg.Password
                    }
                },
                id = 1
            };

            var resp = await _http.PostAsJsonAsync("/jsonrpc", payload);
            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            _uid = doc.RootElement.GetProperty("result").GetInt32();
            _isLogged = true;
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
        public async Task<JsonElement> CallAsync(string service,string rpcMethod,object[] positionalArgs,object? kwargs = null)
        {
            await LoginAsync();

                // Construimos la lista: db, uid, password, modelo, método, parámetros
                var allArgs = new List<object>
            {
                _cfg.Database,
                _uid,
                _cfg.Password
            };
                allArgs.AddRange(positionalArgs);

            var payload = new
            {
                jsonrpc = "2.0",
                method = "call",    // SIEMPRE "call"
                @params = new
                {
                    service = service,     // "object"
                    method = rpcMethod,   // "execute_kw"
                    args = allArgs.ToArray(),
                    kwargs = kwargs ?? new { }
                },
                id = 1
            };

            var resp = await _http.PostAsJsonAsync("/jsonrpc", payload);
            resp.EnsureSuccessStatusCode();

            var content = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var err))
            {
                var msg = err.GetProperty("message").GetString() ?? "Unknown error";
                throw new Exception($"Odoo RPC error: {msg}");
            }

            var resultClone = root.GetProperty("result").Clone();
            return resultClone;
        }

    }

}
