namespace Grupo_G_API.Models
{
    public class PlaylistDetalleDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<CancionEnPlaylistDto> Canciones { get; set; } = new();
    }
}
