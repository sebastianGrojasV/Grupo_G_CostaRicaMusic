using Grupo_G_WEB.Models;

namespace Grupo_G_WEB.Services;

public class InMemoryMusicCatalogService : IMusicCatalogService
{
    private readonly List<Artista> _artistas;
    private readonly List<Album> _albumes;
    private readonly List<Cancion> _canciones;
    private readonly List<Playlist> _playlists;
    private readonly List<PlaylistCancion> _playlistCanciones;

    private int _nextPlaylistId;
    private int _nextPlaylistCancionId;

    public InMemoryMusicCatalogService()
    {
        _artistas =
        [
            new() { Id = 1, Nombre = "Soda Stereo", Biografia = "Banda de rock latino.", FechaCreacion = DateTime.UtcNow.AddYears(-40) },
            new() { Id = 2, Nombre = "Bensound", Biografia = "Catálogo de música royalty-free.", FechaCreacion = DateTime.UtcNow.AddYears(-10) }
        ];

        _albumes =
        [
            new() { Id = 1, Nombre = "Canción Animal", IdArtista = 1, Anio = 1990, FechaCreacion = DateTime.UtcNow.AddYears(-34) },
            new() { Id = 2, Nombre = "Acoustic Set", IdArtista = 2, Anio = 2024, FechaCreacion = DateTime.UtcNow.AddMonths(-6) }
        ];

        _canciones =
        [
            new() { Id = 1, Nombre = "De Música Ligera", IdAlbum = 1, IdArtista = 1, DuracionSegundos = 210, NumeroPista = 1, RutaArchivo = "/audio/de-musica-ligera.mp3", FechaCreacion = DateTime.UtcNow.AddYears(-34) },
            new() { Id = 2, Nombre = "Creative Minds", IdAlbum = 2, IdArtista = 2, DuracionSegundos = 165, NumeroPista = 1, RutaArchivo = "https://www.bensound.com/bensound-music/bensound-creativeminds.mp3", FechaCreacion = DateTime.UtcNow.AddMonths(-6) },
            new() { Id = 3, Nombre = "Energy", IdAlbum = 2, IdArtista = 2, DuracionSegundos = 180, NumeroPista = 2, RutaArchivo = "https://www.bensound.com/bensound-music/bensound-energy.mp3", FechaCreacion = DateTime.UtcNow.AddMonths(-6) }
        ];

        _playlists =
        [
            new() { Id = 1, IdUsuario = 1, Nombre = "Favoritas", Descripcion = "Playlist inicial", FechaCreacion = DateTime.UtcNow }
        ];

        _playlistCanciones =
        [
            new() { Id = 1, IdPlaylist = 1, IdCancion = 2, Orden = 1, FechaAgregado = DateTime.UtcNow },
            new() { Id = 2, IdPlaylist = 1, IdCancion = 3, Orden = 2, FechaAgregado = DateTime.UtcNow }
        ];

        _nextPlaylistId = _playlists.Max(p => p.Id) + 1;
        _nextPlaylistCancionId = _playlistCanciones.Max(pc => pc.Id) + 1;
    }

    public IReadOnlyList<Playlist> GetPlaylists() => _playlists.OrderBy(p => p.Nombre).ToList();

    public PlaylistDetalleDto? GetPlaylistDetalle(int playlistId)
    {
        var playlist = _playlists.FirstOrDefault(p => p.Id == playlistId);
        if (playlist is null)
        {
            return null;
        }

        var canciones = _playlistCanciones
            .Where(pc => pc.IdPlaylist == playlistId)
            .OrderBy(pc => pc.Orden)
            .Select(pc => BuildCancionEnPlaylist(pc))
            .Where(c => c is not null)
            .Cast<CancionEnPlaylistDto>()
            .ToList();

        return new PlaylistDetalleDto
        {
            Id = playlist.Id,
            IdUsuario = playlist.IdUsuario,
            Nombre = playlist.Nombre,
            Descripcion = playlist.Descripcion,
            FechaCreacion = playlist.FechaCreacion,
            Canciones = canciones
        };
    }

    public Playlist CreatePlaylist(int idUsuario, string nombre, string? descripcion)
    {
        var playlist = new Playlist
        {
            Id = _nextPlaylistId++,
            IdUsuario = idUsuario,
            Nombre = nombre,
            Descripcion = descripcion,
            FechaCreacion = DateTime.UtcNow
        };

        _playlists.Add(playlist);
        return playlist;
    }

    public bool AddSongToPlaylist(int playlistId, int cancionId, out string? error)
    {
        error = null;

        if (!_playlists.Any(p => p.Id == playlistId))
        {
            error = "La playlist no existe.";
            return false;
        }

        if (!_canciones.Any(c => c.Id == cancionId))
        {
            error = "La canción no existe.";
            return false;
        }

        if (_playlistCanciones.Any(pc => pc.IdPlaylist == playlistId && pc.IdCancion == cancionId))
        {
            error = "La canción ya se encuentra en la playlist.";
            return false;
        }

        var orden = _playlistCanciones
            .Where(pc => pc.IdPlaylist == playlistId)
            .Select(pc => pc.Orden)
            .DefaultIfEmpty(0)
            .Max() + 1;

        _playlistCanciones.Add(new PlaylistCancion
        {
            Id = _nextPlaylistCancionId++,
            IdPlaylist = playlistId,
            IdCancion = cancionId,
            Orden = orden,
            FechaAgregado = DateTime.UtcNow
        });

        return true;
    }

    public bool RemoveSongFromPlaylist(int playlistId, int cancionId, out string? error)
    {
        error = null;

        var relacion = _playlistCanciones.FirstOrDefault(pc => pc.IdPlaylist == playlistId && pc.IdCancion == cancionId);
        if (relacion is null)
        {
            error = "La canción no está en la playlist indicada.";
            return false;
        }

        _playlistCanciones.Remove(relacion);
        return true;
    }

    public IReadOnlyList<CancionDetalleDto> SearchSongs(string? query)
    {
        var normalized = query?.Trim();

        return _canciones
            .Where(c => string.IsNullOrWhiteSpace(normalized) || c.Nombre.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .Select(MapCancionDetalle)
            .OrderBy(c => c.NombreCancion)
            .ToList();
    }

    public IReadOnlyList<Album> SearchAlbums(string? query)
    {
        var normalized = query?.Trim();

        return _albumes
            .Where(a => string.IsNullOrWhiteSpace(normalized) || a.Nombre.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => a.Nombre)
            .ToList();
    }

    public IReadOnlyList<Artista> SearchArtists(string? query)
    {
        var normalized = query?.Trim();

        return _artistas
            .Where(a => string.IsNullOrWhiteSpace(normalized) || a.Nombre.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => a.Nombre)
            .ToList();
    }

    public CancionDetalleDto? GetSongById(int cancionId)
    {
        var cancion = _canciones.FirstOrDefault(c => c.Id == cancionId);
        return cancion is null ? null : MapCancionDetalle(cancion);
    }

    private CancionEnPlaylistDto? BuildCancionEnPlaylist(PlaylistCancion relacion)
    {
        var cancion = _canciones.FirstOrDefault(c => c.Id == relacion.IdCancion);
        if (cancion is null)
        {
            return null;
        }

        var artista = _artistas.First(a => a.Id == cancion.IdArtista);
        var album = _albumes.First(a => a.Id == cancion.IdAlbum);

        return new CancionEnPlaylistDto
        {
            Id = relacion.Id,
            Orden = relacion.Orden,
            FechaAgregado = relacion.FechaAgregado,
            IdCancion = cancion.Id,
            NombreCancion = cancion.Nombre,
            DuracionSegundos = cancion.DuracionSegundos,
            RutaArchivo = cancion.RutaArchivo,
            IdArtista = artista.Id,
            NombreArtista = artista.Nombre,
            IdAlbum = album.Id,
            NombreAlbum = album.Nombre
        };
    }

    private CancionDetalleDto MapCancionDetalle(Cancion cancion)
    {
        var artista = _artistas.First(a => a.Id == cancion.IdArtista);
        var album = _albumes.First(a => a.Id == cancion.IdAlbum);

        return new CancionDetalleDto
        {
            Id = cancion.Id,
            NombreCancion = cancion.Nombre,
            DuracionSegundos = cancion.DuracionSegundos,
            NumeroPista = cancion.NumeroPista,
            RutaArchivo = cancion.RutaArchivo,
            FechaCreacion = cancion.FechaCreacion,
            IdArtista = artista.Id,
            NombreArtista = artista.Nombre,
            BiografiaArtista = artista.Biografia,
            UrlImagenArtista = artista.UrlImagen,
            IdAlbum = album.Id,
            NombreAlbum = album.Nombre,
            AnioAlbum = album.Anio,
            UrlPortadaAlbum = album.UrlPortada
        };
    }
}
