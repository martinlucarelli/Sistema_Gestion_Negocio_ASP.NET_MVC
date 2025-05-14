using Microsoft.AspNetCore.Http.HttpResults;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Services
{
    public class ProductoApiService : IProductoApiService
    {
        NegocioContext context;
        public readonly ILogger<ProductoApiService> logger;

        
        
        public ProductoApiService(NegocioContext _context, ILogger<ProductoApiService> _logger)
        {
            context= _context;
            logger= _logger;
        }

        public IEnumerable<Producto> Get(Guid idNegocio)
        { 
            return context.Productos.Where(p => p.NegocioId == idNegocio);
        }

        public Producto GetProductoDetalle(string id)
        {
            return context.Productos.Find(id);
        }

        public async Task Save(ProductoDTO nuevoProducto)
        {
            var producto = new Producto
            {
                Nombre=nuevoProducto.Nombre,
                Precio=nuevoProducto.Precio.Value,
                Stock=nuevoProducto.Stock.Value,
                NegocioId=nuevoProducto.NegocioId
            };
            
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
        }
        public async Task Delete(string id)
        {
            var productoEliminar = await context.Productos.FindAsync(id);
            
            if(productoEliminar != null)
            {
                context.Productos.Remove(productoEliminar);
            }
            else
            {
                logger.LogError("NO SE ENCONTRO EL PRODUCTO QUE SE DESEA ELIMINAR");
            }
            
            
            await context.SaveChangesAsync();
        }
        public async Task Update(string id,ProductoDTO productoUpd)
        {
            var productoExistente = await context.Productos.FindAsync(id);

            if(productoExistente != null)
            {
                if(!string.IsNullOrEmpty(productoUpd.Nombre))
                {
                    productoExistente.Nombre = productoUpd.Nombre;
                }
                if (productoUpd.Precio.HasValue && productoUpd.Precio > 0 && productoUpd.Precio < 1000000000)
                {
                    productoExistente.Precio = productoUpd.Precio.Value;

                    logger.LogError("ERROR AL CAMBIAR PRECIO DEL PRODUCTO");
                }
                if (productoUpd.Stock.HasValue)
                {
                    productoExistente.Stock = productoUpd.Stock.Value;
                }
            }
            else
            {
                logger.LogError("NO SE ENCONTRO EL PRODUCTO QUE SE DESEA ACTUALIZAR");
            }
            
            await context.SaveChangesAsync();
        }

    }



    public interface IProductoApiService
    {

        IEnumerable<Producto> Get(Guid idNegocio);
        Producto GetProductoDetalle(string id);
        Task Save(ProductoDTO nuevoProducto);
        Task Delete(string id);
        Task Update(string id,ProductoDTO productoUpd);
    }
}
