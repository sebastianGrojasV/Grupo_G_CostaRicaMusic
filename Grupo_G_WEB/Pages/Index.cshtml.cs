using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Grupo_G_WEB.Pages;

public class IndexModel : PageModel
{
    public IReadOnlyList<CancionDemo> Canciones { get; } =
    [
        new("Bensound - Creative Minds", "Bensound", "https://www.bensound.com/bensound-music/bensound-creativeminds.mp3"),
        new("Bensound - Jazzy Frenchy", "Bensound", "https://www.bensound.com/bensound-music/bensound-jazzyfrenchy.mp3"),
        new("Bensound - Energy", "Bensound", "https://www.bensound.com/bensound-music/bensound-energy.mp3")
    ];

    public void OnGet()
    {
    }

    public sealed record CancionDemo(string Titulo, string Artista, string UrlPreview);
}
