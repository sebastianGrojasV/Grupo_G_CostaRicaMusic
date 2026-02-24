using Grupo_G_WEB.Models.Api;
using Grupo_G_WEB.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IMusicCatalogService, InMemoryMusicCatalogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

var api = app.MapGroup("/api");

api.MapGet("/search", (string? q, string? tipo, IMusicCatalogService catalogo) =>
{
    var kind = (tipo ?? "todo").Trim().ToLowerInvariant();

    return Results.Ok(new
    {
        query = q,
        canciones = kind is "todo" or "canciones" ? catalogo.SearchSongs(q) : Array.Empty<Grupo_G_WEB.Models.CancionDetalleDto>(),
        albumes = kind is "todo" or "albumes" ? catalogo.SearchAlbums(q) : Array.Empty<Grupo_G_WEB.Models.Album>(),
        artistas = kind is "todo" or "artistas" ? catalogo.SearchArtists(q) : Array.Empty<Grupo_G_WEB.Models.Artista>()
    });
});

api.MapGet("/canciones/{id:int}", (int id, IMusicCatalogService catalogo) =>
{
    var song = catalogo.GetSongById(id);
    return song is null ? Results.NotFound(new { mensaje = "Canción no encontrada." }) : Results.Ok(song);
});

api.MapGet("/canciones/{id:int}/reproduccion", (int id, IMusicCatalogService catalogo) =>
{
    var song = catalogo.GetSongById(id);
    if (song is null)
    {
        return Results.NotFound(new { mensaje = "Canción no encontrada." });
    }

    return Results.Ok(new
    {
        id = song.Id,
        nombre = song.NombreCancion,
        urlReproduccion = song.RutaArchivo,
        duracionSegundos = song.DuracionSegundos
    });
});

api.MapGet("/playlists", (IMusicCatalogService catalogo) => Results.Ok(catalogo.GetPlaylists()));

api.MapGet("/playlists/{id:int}", (int id, IMusicCatalogService catalogo) =>
{
    var playlist = catalogo.GetPlaylistDetalle(id);
    return playlist is null ? Results.NotFound(new { mensaje = "Playlist no encontrada." }) : Results.Ok(playlist);
});

api.MapPost("/playlists", (CreatePlaylistRequest request, IMusicCatalogService catalogo) =>
{
    if (request.IdUsuario <= 0 || string.IsNullOrWhiteSpace(request.Nombre))
    {
        return Results.BadRequest(new { mensaje = "IdUsuario y Nombre son requeridos." });
    }

    var playlist = catalogo.CreatePlaylist(request.IdUsuario, request.Nombre.Trim(), request.Descripcion?.Trim());
    return Results.Created($"/api/playlists/{playlist.Id}", playlist);
});

api.MapPost("/playlists/{id:int}/canciones", (int id, AddSongToPlaylistRequest request, IMusicCatalogService catalogo) =>
{
    if (request.IdCancion <= 0)
    {
        return Results.BadRequest(new { mensaje = "IdCancion debe ser mayor a cero." });
    }

    var added = catalogo.AddSongToPlaylist(id, request.IdCancion, out var error);
    if (!added)
    {
        return Results.BadRequest(new { mensaje = error });
    }

    var detalle = catalogo.GetPlaylistDetalle(id);
    return Results.Ok(detalle);
});

api.MapDelete("/playlists/{id:int}/canciones/{idCancion:int}", (int id, int idCancion, IMusicCatalogService catalogo) =>
{
    var removed = catalogo.RemoveSongFromPlaylist(id, idCancion, out var error);
    if (!removed)
    {
        return Results.NotFound(new { mensaje = error });
    }

    return Results.NoContent();
});

app.MapRazorPages();

app.Run();
