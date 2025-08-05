using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Services
{
    public class NegocioApiService : INegocioApiService
    {
        NegocioContext context;
        public readonly ILogger<NegocioApiService> logger;

        public NegocioApiService(NegocioContext _context, ILogger<NegocioApiService> _logger)
        {
            context = _context;
            logger = _logger;
        }


        public IEnumerable<UsuarioDTO> GetUsuariosNegocio(Guid IdNegocio)
        {
            return context.Usuarios.Where(u=> u.NegocioId == IdNegocio)
                .Select(u=> new UsuarioDTO
                {
                    nombre= u.Nombre,
                    correo = u.Correo,
                    rol = u.Rol
                }).ToList();
        }
        public async Task EliminarNegocio(Guid IdNegocio)
        {


            var ventas = await context.Ventas.Where(v => v.NegocioId == IdNegocio).ToListAsync();

            foreach(var venta in ventas)
            {
                var detalles = await context.DetalleVentas.Where(d=> d.VentaId == venta.IdVenta).ToListAsync();

                context.DetalleVentas.RemoveRange(detalles); //Elimina toda la lista, sin necesidad de iterar y eliminar uno por uno

                context.Remove(venta);
            }

            var productos = await context.Productos.Where(p => p.NegocioId == IdNegocio).ToListAsync();

            foreach(var producto in productos)
            {
                context.Productos.Remove(producto);
            }

            var usuarios = await context.Usuarios.Where(u => u.NegocioId == IdNegocio).ToListAsync();

            context.Usuarios.RemoveRange(usuarios);

            var negocio = await context.Negocios.FirstOrDefaultAsync(n=> n.IdNegocio == IdNegocio);

            context.Negocios.Remove(negocio);

            await context.SaveChangesAsync();



        }




    }

    public interface INegocioApiService 
    {
        IEnumerable<UsuarioDTO> GetUsuariosNegocio(Guid IdNegocio);
        Task EliminarNegocio(Guid IdNegocio);
    
    }
}
