namespace Grupo_G_API.Models
{
    public class PlaylistCancion
    {
        public int Id { get; set; }
        public int IdPlaylist { get; set; }
        public int IdCancion { get; set; }
        public int Orden { get; set; }
        public DateTime FechaAgregado { get; set; }
    }
}
