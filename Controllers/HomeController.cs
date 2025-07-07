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



        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AccesoDenegado() 
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
