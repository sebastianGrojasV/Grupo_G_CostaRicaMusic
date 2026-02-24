using Grupo_G_WEB.Models;
using Grupo_G_WEB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Grupo_G_WEB.Pages;

public class SearchModel(IMusicCatalogService catalogService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Type { get; set; }

    public IReadOnlyList<CancionDetalleDto> Canciones { get; private set; } = [];
    public IReadOnlyList<Artista> Artistas { get; private set; } = [];
    public IReadOnlyList<Album> Albumes { get; private set; } = [];

    public CancionDetalleDto? TopResultSong => Canciones.FirstOrDefault();

    public void OnGet()
    {
        Canciones = catalogService.SearchSongs(Q);
        Artistas = catalogService.SearchArtists(Q);
        Albumes = catalogService.SearchAlbums(Q);

        var filter = Type?.Trim().ToLowerInvariant();

        switch (filter)
        {
            case "songs":
                Artistas = [];
                Albumes = [];
                break;
            case "artists":
                Canciones = [];
                Albumes = [];
                break;
            case "albums":
                Canciones = [];
                Artistas = [];
                break;
        }
    }
}
