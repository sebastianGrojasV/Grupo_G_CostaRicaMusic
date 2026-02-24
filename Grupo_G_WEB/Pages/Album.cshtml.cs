using Grupo_G_WEB.Models;
using Grupo_G_WEB.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Grupo_G_WEB.Pages;

public class AlbumModel(IMusicCatalogService catalogService) : PageModel
{
    public Album? Album { get; private set; }

    public void OnGet(int id)
    {
        Album = catalogService.SearchAlbums(null).FirstOrDefault(a => a.Id == id);
    }
}
