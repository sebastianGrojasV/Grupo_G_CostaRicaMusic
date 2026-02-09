using System.Security.Claims;

namespace Grupo_G_API.Dependencias
{
    public class Utilitarios : IUtilitarios
    {
        public long ObtenerUsuarioFromToken(IEnumerable<Claim> valores)
        {
            if (valores.Any())
            {
                var idUsuario = valores.FirstOrDefault(x => x.Type == "Id_Usuario")?.Value;
                return long.Parse(idUsuario!);
            }

            return 0;
        }

        public bool ValidarUsuarioAdministradorFromToken(IEnumerable<Claim> valores)
        {
            if (valores.Any())
            {
                var idPerfil = valores.FirstOrDefault(x => x.Type == "Id_Perfil")?.Value;
                return idPerfil == "2";
            }

            return false;
        }
    }
}
