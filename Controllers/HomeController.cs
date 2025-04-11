using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using System.Diagnostics;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    [Authorize] //Cualquier tipo de usuario necesita tener autorizacion para acceder a etas paginas, para ello debe estar logueado
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "AdministradorGeneral,AdministradorNegocio,Empleado")] //Acceden los 3 tipos de usuarios
        public IActionResult EpleadoVista_Prueba()
        {
            return View();
        }
        [Authorize(Roles = "AdministradorGeneral,AdministradorNegocio")] //Solo 2 usuarios tiene acceso
        public IActionResult AdminNegocioVista_Prueba()
        {
            return View();
        }

        [Authorize(Roles = "AdministradorGeneral")] //Solo un usuario tiene acceso
        public IActionResult AdminGeneralVista_Prueba()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
