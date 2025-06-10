using Microsoft.EntityFrameworkCore;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Services
{
    public class VentaApiService : IVentaApiService
    {

        NegocioContext context;
        public readonly ILogger<VentaApiService> logger;

        public VentaApiService(NegocioContext _context, ILogger<VentaApiService> _logger)
        {
            context = _context;
            logger = _logger;
        }



        public ResumenVentasDTO Get(Guid idNegocio, DateTime? desde, DateTime? hasta)
        {
            var query = context.Ventas
                .Where(v => v.NegocioId == idNegocio)
                .Include(v => v.FormaPago)
                .Include(v => v.Usuario)
                .AsQueryable();

            if (desde.HasValue && hasta.HasValue)
            {
                var desdeFecha = desde.Value.Date;
                var hastaFecha = hasta.Value.Date.AddDays(1); // Incluye todo el día 'hasta'
                query = query.Where(v => v.Fecha >= desdeFecha && v.Fecha < hastaFecha);
            }
            else
            {
                // Ventas solo del día actual
                var hoy = DateTime.Today; // 2025-05-19 00:00:00
                var mañana = hoy.AddDays(1); // 2025-05-20 00:00:00

                query = query.Where(v => v.Fecha >= hoy && v.Fecha < mañana);

            }

            var ventas = query.ToList();

            var resumenPorFormaPago = ventas
                .GroupBy(v => v.FormaPago?.TipoFormaPago ?? "Sin forma de pago")
                .Select(g => new ResumenFormaPagoDTO
                {
                    FormaPago = g.Key,
                    Total = g.Sum(v => v.Total)
                }).ToList();

            var totalGeneral = ventas.Sum(v => v.Total);

            return new ResumenVentasDTO
            {
                Ventas = ventas.Select(v => new MostrarVentaDTO
                {
                    IdVenta = v.IdVenta,
                    Fecha = v.Fecha,
                    Total = v.Total,
                    NombreFormaPago = v.FormaPago != null ? v.FormaPago.TipoFormaPago : "Sin forma de pago",
                    NombreUsuario = v.Usuario != null ? v.Usuario.Nombre : "Usuario desconocido"
                }).ToList(),
                ResumenPorFormaPago = resumenPorFormaPago,
                TotalGeneral = totalGeneral
            };
        }

        public FacturaDTO GetFactura(string idVenta)
        {
            
            var venta = context.Ventas
                .Include(v => v.detalleVentas)
                    .ThenInclude(dv => dv.producto)
                .Include(v => v.FormaPago)
                .Include(v => v.Usuario)
                .Include(v => v.Negocio)
                .FirstOrDefault(v => v.IdVenta == idVenta);

            if (venta == null) return null;



            return new FacturaDTO
            {
                IdVenta = venta.IdVenta,
                fechaVenta = venta.Fecha,
                FormaPago = venta.FormaPago?.TipoFormaPago,
                Total = venta.Total,
                Vendedor = venta.Usuario?.Nombre,
                NombreNegocio = venta.Negocio?.Nombre,
                DireccionNegocio = venta.Negocio?.Direccion,
                Detalle = venta.detalleVentas?.Select(d => new DetalleVentaDTO
                {
                    ProductoId = d.ProductoId,
                    CantidadProductos = d.CantidadProductos,
                    Subtotal = d.Subtotal,
                    PrecioProducto = d.producto?.Precio ?? 0,
                    NombreProducto= d.producto.Nombre
                }).ToList()
            };
        }

        public async Task Registrar(VentaDTO nuevaVenta)
        {

            var venta = new Venta
            {
                UsuarioId = nuevaVenta.UsuarioId,
                NegocioId = nuevaVenta.NegocioId,
                FormaPagoId = nuevaVenta.FormaPagoId,
                Total = nuevaVenta.Total,
            };

            context.Ventas.Add(venta);

            foreach (var detalleDeVenta in nuevaVenta.Detalle)
            {
                var detalleVenta = new DetalleVenta
                {
                    CantidadProductos = detalleDeVenta.CantidadProductos,
                    Subtotal = detalleDeVenta.Subtotal,
                    VentaId = venta.IdVenta,
                    ProductoId = detalleDeVenta.ProductoId
                };

                context.DetalleVentas.Add(detalleVenta);
            }

            
            await context.SaveChangesAsync();
        }
    }



    public interface IVentaApiService
    {
       ResumenVentasDTO Get(Guid idNegocio, DateTime? desde, DateTime? hasta);
        FacturaDTO GetFactura(string idVenta);
        Task Registrar(VentaDTO nuevaVenta);
    }

}
