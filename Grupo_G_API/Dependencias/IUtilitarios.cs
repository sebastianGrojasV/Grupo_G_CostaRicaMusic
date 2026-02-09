using System.Security.Claims;

namespace Grupo_G_API.Dependencias
{
    public interface IUtilitarios
    {
        long ObtenerUsuarioFromToken(IEnumerable<Claim> valores);
        bool ValidarUsuarioAdministradorFromToken(IEnumerable<Claim> valores);
    }
}
