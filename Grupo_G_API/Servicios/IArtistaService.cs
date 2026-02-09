using Grupo_G_API.Models;

namespace Grupo_G_API.Servicios
{
    public interface IArtistaService
    {
        Task<List<Artista>> ListarAsync();
        Task<Artista?> ObtenerPorIdAsync(int id);
        Task<List<ArtistaCancionItemDto>> ListarCancionesPorArtistaAsync(int idArtista);
    }
}
