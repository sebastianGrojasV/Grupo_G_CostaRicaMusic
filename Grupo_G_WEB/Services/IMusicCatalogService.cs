using Grupo_G_WEB.Models;

namespace Grupo_G_WEB.Services;

public interface IMusicCatalogService
{
    IReadOnlyList<Playlist> GetPlaylists();
    PlaylistDetalleDto? GetPlaylistDetalle(int playlistId);
    Playlist CreatePlaylist(int idUsuario, string nombre, string? descripcion);
    bool AddSongToPlaylist(int playlistId, int cancionId, out string? error);
    bool RemoveSongFromPlaylist(int playlistId, int cancionId, out string? error);

    IReadOnlyList<CancionDetalleDto> SearchSongs(string? query);
    IReadOnlyList<Album> SearchAlbums(string? query);
    IReadOnlyList<Artista> SearchArtists(string? query);

    CancionDetalleDto? GetSongById(int cancionId);
}
