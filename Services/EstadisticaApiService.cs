using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Services
{
    public class EstadisticaApiService : IEstadisticaApiService
    {
        NegocioContext context;
        public readonly ILogger<EstadisticaApiService> logger;

        public EstadisticaApiService(NegocioContext _context, ILogger<EstadisticaApiService> _logger)
        {
            context = _context;
            logger = _logger;
        }

        private static DateTime establecerFechaPorTiempo (Tiempo t)
        {
            DateTime fechaHasta = DateTime.Today.AddDays(1); // mañana para incluir hoy completo
            DateTime fechaDesde;

            // Establecer fechaDesde según el tiempo
            int valor = (int)t;

            if(valor >= 1 && valor <=12)
            {
                int anioActual = DateTime.Today.Year;
                fechaDesde = new DateTime(anioActual, valor, 1);
                fechaHasta = fechaDesde.AddMonths(1);
                return fechaDesde;
            }
            else
            {
                switch (t)
                {
                    case Tiempo.ultimoDia:
                        fechaDesde = DateTime.Today;
                        break;

                    case Tiempo.ultimaSemana:
                        int diasDesdeLunes = ((int)DateTime.Today.DayOfWeek + 6) % 7;
                        fechaDesde = DateTime.Today.AddDays(-diasDesdeLunes); // lunes de esta semana
                        break;

                    case Tiempo.ultimoMes:
                        fechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        break;

                    case Tiempo.ultimos3Meses:
                        fechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-2);
                        break;

                    case Tiempo.ultimos6Meses:
                        fechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);
                        break;

                    case Tiempo.ultimoAnio:
                        fechaDesde = new DateTime(DateTime.Today.Year, 1, 1);
                        break;

                    default:
                        fechaDesde = DateTime.Today;
                        break;
                }

                return fechaDesde;
            }


        }


        public EstadisticaDTO GetEstadisticas(Guid idNegocio,Tiempo tiempo)
        {
            DateTime fechaHasta = DateTime.Today.AddDays(1); // mañana para incluir hoy completo
            DateTime fechaDesde = establecerFechaPorTiempo(tiempo);
            int valor = (int)tiempo;

            if (valor >= 1 && valor <= 12)
            {
                fechaHasta = fechaDesde.AddMonths(1);

                if(fechaDesde > DateTime.Today)
                {
                    return new EstadisticaDTO
                    {
                        VentasPorPeriodo = new(),
                        GananciasPorPeriodo = new(),
                        ProductosMasVendidos = new(),
                        FormasDePago = new()
                    };
                }

            }



            // Obtener ventas
            var ventas = context.Ventas
                .Where(v => v.NegocioId == idNegocio && v.Fecha >= fechaDesde && v.Fecha < fechaHasta)
                .Include(v => v.detalleVentas)
                    .ThenInclude(dv => dv.producto)
                .Include(v => v.FormaPago)
                .ToList();

            List<ItemGrafico> listaVentasPorPeriodo = new();
            List<ItemGrafico> listaGananciasPorPeriodo = new();

            // Generar períodos completos
            if (tiempo == Tiempo.ultimoDia)
            {
                var horas = Enumerable.Range(0, 24)
                    .Select(h => DateTime.Today.AddHours(h))
                    .ToList();

                listaVentasPorPeriodo = horas.Select(h => new ItemGrafico
                {
                    Periodo = h.ToString("HH:mm"),
                    Valor = ventas.Count(v => v.Fecha.Hour == h.Hour)
                }).ToList();

                listaGananciasPorPeriodo = horas.Select(h => new ItemGrafico
                {
                    Periodo = h.ToString("HH:mm"),
                    Valor = ventas.Where(v => v.Fecha.Hour == h.Hour).Sum(v => v.Total)
                }).ToList();
            }

            else if (tiempo == Tiempo.ultimaSemana)
            {
                var diasSemana = Enumerable.Range(0, (DateTime.Today - fechaDesde).Days + 1)
                    .Select(i => fechaDesde.AddDays(i))
                    .ToList();

                listaVentasPorPeriodo = diasSemana.Select(d => new ItemGrafico
                {
                    Periodo = d.ToString("dddd"),
                    Valor = ventas.Count(v => v.Fecha.Date == d.Date)
                }).ToList();

                listaGananciasPorPeriodo = diasSemana.Select(d => new ItemGrafico
                {
                    Periodo = d.ToString("dddd"),
                    Valor = ventas.Where(v => v.Fecha.Date == d.Date).Sum(v => v.Total)
                }).ToList();
            }

            else if (tiempo == Tiempo.ultimoMes || (valor >=1 && valor <=12))
            {
                var diasDelMes = Enumerable.Range(0, (fechaHasta - fechaDesde).Days )
                    .Select(i => fechaDesde.AddDays(i))
                    .ToList();

                listaVentasPorPeriodo = diasDelMes.Select(d => new ItemGrafico
                {
                    Periodo = d.ToString("dd/MM"),
                    Valor = ventas.Count(v => v.Fecha.Date == d.Date)
                }).ToList();

                listaGananciasPorPeriodo = diasDelMes.Select(d => new ItemGrafico
                {
                    Periodo = d.ToString("dd/MM"),
                    Valor = ventas.Where(v => v.Fecha.Date == d.Date).Sum(v => v.Total)
                }).ToList();
            }

            else
            {
                // Para últimos 3, 6 o 12 meses
                int cantidadMeses = tiempo switch
                {
                    Tiempo.ultimos3Meses => 3,
                    Tiempo.ultimos6Meses => 6,
                    Tiempo.ultimoAnio => 12,
                    _ => 3
                };

                var primerMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-(cantidadMeses - 1));
                var meses = Enumerable.Range(0, cantidadMeses)
                    .Select(i => primerMes.AddMonths(i))
                    .ToList();

                listaVentasPorPeriodo = meses.Select(m => new ItemGrafico
                {
                    Periodo = m.ToString("MMM yyyy"),
                    Valor = ventas.Count(v => v.Fecha.Year == m.Year && v.Fecha.Month == m.Month)
                }).ToList();

                listaGananciasPorPeriodo = meses.Select(m => new ItemGrafico
                {
                    Periodo = m.ToString("MMM yyyy"),
                    Valor = ventas.Where(v => v.Fecha.Year == m.Year && v.Fecha.Month == m.Month).Sum(v => v.Total)
                }).ToList();
            }

            // Productos más vendidos
            var productosMasVendidos = ventas
                .SelectMany(v => v.detalleVentas)
                .GroupBy(d => d.ProductoId)
                .Select(g => new ProductoEstadistica
                {
                    Nombre = g.First().producto?.Nombre ?? "(Producto eliminado)",
                    CantidadVendida = g.Sum(x => x.CantidadProductos)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(5)
                .ToList();

            // Formas de pago
            var formasPago = ventas
                .GroupBy(v => v.FormaPagoId)
                .Select(g => new FormaPagoEstadistica
                {
                    Metodo = g.First().FormaPago.TipoFormaPago,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();

            
            
            return new EstadisticaDTO
            {
                VentasPorPeriodo = listaVentasPorPeriodo,
                GananciasPorPeriodo = listaGananciasPorPeriodo,
                ProductosMasVendidos = productosMasVendidos,
                FormasDePago = formasPago
            };

        }

    }

    public interface IEstadisticaApiService
    {

        EstadisticaDTO GetEstadisticas(Guid idNegocio, Tiempo tiempo);

    }


}
