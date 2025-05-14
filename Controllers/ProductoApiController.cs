using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ProductoApiController : ControllerBase
    {
        IProductoApiService productoApiService;
        NegocioContext context;
        public readonly ILogger<ProductoApiController> logger;

        public ProductoApiController(IProductoApiService _productoApiService, NegocioContext _context, ILogger<ProductoApiController> _logger)
        {
            productoApiService = _productoApiService;
            context = _context;
            logger = _logger;
        }

        [HttpGet("{idNegocio}")]
        public ActionResult Get(Guid idNegocio)
        {
            return Ok(productoApiService.Get(idNegocio));

        }
        [HttpGet("Detalle/{id}")]
        public IActionResult GetProductoDetalle(string id)
        {
            var producto = productoApiService.GetProductoDetalle(id);

            if (producto == null)
            {
                return NotFound();
            }

            var dto = new ProductoDTO
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock,
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductoDTO nuevoProducto)
        {
            await productoApiService.Save(nuevoProducto);
            return Ok(new { message = "Producto guardado con éxito" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await productoApiService.Delete(id);
            return Ok(new { message = "Producto eliminado" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] ProductoDTO productoUpd)
        {
            var prod = context.Productos.FirstOrDefault(p=> p.IdProducto== id);

            if(prod ==null)
            {  
                return NotFound(); 
            }
            else
            { 
                await productoApiService.Update(id, productoUpd);
                return Ok();
            }

        }



    }
}
