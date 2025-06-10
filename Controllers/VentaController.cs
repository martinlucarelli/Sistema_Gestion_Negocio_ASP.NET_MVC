using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Helper;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    public class VentaController : Controller
    {

        NegocioContext context;
        public ILogger<LoginController> logger;

        public VentaController(NegocioContext _context, ILogger<LoginController> _logger)
        {
            context = _context;
            logger = _logger;
        }



        public IActionResult RegistrarVenta()
        {
            string negocioIdUsuario = UsuarioHelper.ObtenerNegocioIdDelUsuario(HttpContext);
            string usuarioId = UsuarioHelper.ObtenerUsuarioId(HttpContext);

            ViewBag.negocioId = negocioIdUsuario;
            ViewBag.usuarioId = usuarioId;
            ViewBag.formasDePago = context.FormasPago.ToList();
            return View();
        }

        public IActionResult Ventas()
        {
            string negocioIdUsuario = UsuarioHelper.ObtenerNegocioIdDelUsuario(HttpContext);

            ViewBag.negocioId = negocioIdUsuario;

            return View();
        }
    }
}
