using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class NegocioApiController : Controller
    {
        INegocioApiService negocioApiService;
        NegocioContext context;
        public readonly ILogger<NegocioApiController> logger;

        public NegocioApiController(INegocioApiService _negocioApiService, NegocioContext _context, ILogger<NegocioApiController> _logger)
        {
            negocioApiService = _negocioApiService;
            context = _context;
            logger = _logger;
        }


        [HttpGet("{IdNegocio}")]
        public ActionResult GetUsuarioNegocio(Guid IdNegocio)
        {
            return Ok(negocioApiService.GetUsuariosNegocio(IdNegocio));
        }

        [HttpDelete("{IdNegocio}")]
        public async Task<ActionResult> EliminarNegocio(Guid IdNegocio)
        {
            await negocioApiService.EliminarNegocio(IdNegocio);
            return Ok();
        }

    }
}
