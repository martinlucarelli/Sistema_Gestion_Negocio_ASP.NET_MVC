using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Helper;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    public class EstadisticasController : Controller
    {
        NegocioContext context;
        public ILogger<EstadisticasController> logger;

        public EstadisticasController(NegocioContext _context, ILogger<EstadisticasController> _logger)
        {
            context = _context;
            logger = _logger;
        }


        public IActionResult mostrarEstadisticas()
        {

            string id = UsuarioHelper.ObtenerNegocioIdDelUsuario(HttpContext);

            ViewBag.negocioId = id;

            return View();
        }

    }
}
