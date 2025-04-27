using System.Security.Claims;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Helper
{
    public static class UsuarioHelper
    {

        public static string ObtenerUsuarioId(HttpContext context)
        {
            var idClaim= context.User.Claims.FirstOrDefault(c=> c.Type == ClaimTypes.NameIdentifier);

            return idClaim?.Value;


        }


    }
}
