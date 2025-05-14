using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Helper;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    public class ProductoController : Controller
    {
        NegocioContext context;
        public ILogger<LoginController> logger;

        public ProductoController (NegocioContext _context, ILogger<LoginController> _logger)
        {
            context = _context;
            logger = _logger;
        }

        public IActionResult Productos()
        {
            string negocioIdUsuario = UsuarioHelper.ObtenerNegocioIdDelUsuario(HttpContext);

            ViewBag.negocioId = negocioIdUsuario;

            return View();
        }
    }
}
