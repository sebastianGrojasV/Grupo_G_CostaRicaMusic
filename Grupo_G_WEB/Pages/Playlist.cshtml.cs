using Grupo_G_WEB.Models;
using Grupo_G_WEB.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Grupo_G_WEB.Pages;

public class PlaylistModel(IMusicCatalogService catalogService) : PageModel
{
    public PlaylistDetalleDto? Playlist { get; private set; }

    public void OnGet(int id)
    {
        Playlist = catalogService.GetPlaylistDetalle(id);
    }
}
