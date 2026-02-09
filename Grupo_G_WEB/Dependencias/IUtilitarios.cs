using Grupo_G_WEB.Models;

namespace Grupo_G_WEB.Dependencias
{
    public interface IUtilitarios
    {
        List<Cancion> ConsultarCanciones(int pagina, int filasPorPagina, string? nombreFiltro);
        List<Playlist> ConsultarPlaylistsPorUsuario(int idUsuario);
    }
}
