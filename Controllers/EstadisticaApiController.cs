using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class EstadisticaApiController : Controller
    {
        IEstadisticaApiService estadisticaApiService;
        NegocioContext context;
        public ILogger<EstadisticaApiController> logger;

        public EstadisticaApiController(NegocioContext _context, ILogger<EstadisticaApiController> _logger, IEstadisticaApiService _estadisticaApiService)
        {
            context = _context;
            logger = _logger;
            estadisticaApiService = _estadisticaApiService;
        }

        [HttpGet("{idNegocio}")]
        public IActionResult GetEstadistica(Guid idNegocio, Tiempo tiempo)
        {
            var estadistica = estadisticaApiService.GetEstadisticas(idNegocio, tiempo);
            
            return Ok(estadistica);
        }
    }
}
