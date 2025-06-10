using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Helper;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class VentaApiController : Controller
    {
        IVentaApiService ventaApiService;
        NegocioContext context;
        public ILogger<LoginController> logger;

        public VentaApiController(NegocioContext _context, ILogger<LoginController> _logger,IVentaApiService _ventaApiService)
        {
            context = _context;
            logger = _logger;
            ventaApiService = _ventaApiService;
        }



        [HttpGet ("{idNegocio}")]
        public ActionResult Get(Guid idNegocio, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            var resumenVentas = ventaApiService.Get(idNegocio, desde, hasta);
            return Ok(resumenVentas);
        }

        [HttpGet("factura/{idVenta}")]
        public ActionResult GetFactura(string idVenta)
        {
            var factura = ventaApiService.GetFactura(idVenta);

            if(factura == null)
            {
                return NotFound();
            }
            else
            {
                var pdfBytes = CrearPDFHelper.crearPDFFactura(factura);

                return File(pdfBytes, "application/pdf");
                
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registar([FromBody] VentaDTO nuevaVenta)
        {
            await ventaApiService.Registrar(nuevaVenta);
            return Ok();
        }
    }
}
