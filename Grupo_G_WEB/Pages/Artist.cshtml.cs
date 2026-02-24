using Grupo_G_WEB.Models;
using Grupo_G_WEB.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Grupo_G_WEB.Pages;

public class ArtistModel(IMusicCatalogService catalogService) : PageModel
{
    public Artista? Artista { get; private set; }

    public void OnGet(int id)
    {
        Artista = catalogService.SearchArtists(null).FirstOrDefault(a => a.Id == id);
    }
}
