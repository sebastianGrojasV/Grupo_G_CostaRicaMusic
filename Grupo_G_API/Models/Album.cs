namespace Grupo_G_API.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdArtista { get; set; }
        public int? Anio { get; set; }
        public string? UrlPortada { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
