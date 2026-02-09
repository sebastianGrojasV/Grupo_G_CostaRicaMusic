namespace Grupo_G_API.Models
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Token { get; set; }
    }
}
