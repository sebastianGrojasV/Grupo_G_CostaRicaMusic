namespace Grupo_G_API.Models
{
    public class Artista
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Biografia { get; set; }
        public string? UrlImagen { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
