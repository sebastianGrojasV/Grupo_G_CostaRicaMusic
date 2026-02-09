using System.Net.Http.Headers;
using System.Text.Json;
using Grupo_G_WEB.Models;
using Microsoft.Extensions.Configuration;

namespace Grupo_G_WEB.Dependencias
{
    public class Utilitarios : IUtilitarios
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;

        public Utilitarios(IHttpClientFactory httpClient, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _accessor = accessor;
        }

        public List<Cancion> ConsultarCanciones(int pagina, int filasPorPagina, string? nombreFiltro)
        {
            using (var api = _httpClient.CreateClient())
            {
                var baseUrl = _configuration.GetSection("Variables:urlApi").Value?.TrimEnd('/');
                var url = $"{baseUrl}/Canciones/Listar?Pagina={pagina}&FilasPorPagina={filasPorPagina}&NombreFiltro={Uri.EscapeDataString(nombreFiltro ?? "")}";

                var token = _accessor.HttpContext?.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                    api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = api.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;
                    if (result != null && result.Indicador && result.Datos != null)
                        return JsonSerializer.Deserialize<List<Cancion>>(((JsonElement)result.Datos).GetRawText()) ?? new List<Cancion>();
                }
            }

            return new List<Cancion>();
        }

        public List<Playlist> ConsultarPlaylistsPorUsuario(int idUsuario)
        {
            using (var api = _httpClient.CreateClient())
            {
                var baseUrl = _configuration.GetSection("Variables:urlApi").Value?.TrimEnd('/');
                var url = $"{baseUrl}/Playlists/ListarPorUsuario?IdUsuario={idUsuario}";

                var token = _accessor.HttpContext?.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                    api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = api.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;
                    if (result != null && result.Indicador && result.Datos != null)
                        return JsonSerializer.Deserialize<List<Playlist>>(((JsonElement)result.Datos).GetRawText()) ?? new List<Playlist>();
                }
            }

            return new List<Playlist>();
        }
    }
}
