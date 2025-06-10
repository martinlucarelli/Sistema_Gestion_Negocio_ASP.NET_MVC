namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO
{
    public class VentaDTO
    {
        public Guid UsuarioId { get; set; }
        public Guid NegocioId { get; set; }
        public string? IdVenta { get; set; }
        public int FormaPagoId { get; set; }
        public double Total { get; set; }

        public DateTime? Fecha { get; set; }

        //Para la vista
        public string? NombreUsuario { get; set; }
        public string? NombreFormaPago {  get; set; }

        public List<DetalleVentaDTO>? Detalle { get; set; }
    }

    public class MostrarVentaDTO
    {
        public string? IdVenta { get; set; }
        public double Total { get; set; }
        public DateTime? Fecha { get; set; }
        public string? NombreUsuario { get; set; }
        public string? NombreFormaPago { get; set; }
    }

    public class ResumenVentasDTO
    {
        public List<MostrarVentaDTO> Ventas { get; set; }
        public List<ResumenFormaPagoDTO> ResumenPorFormaPago { get; set; }
        public double TotalGeneral { get; set; }

    }

    public class ResumenFormaPagoDTO
    {
        public string FormaPago { get; set; }
        public double Total {  get; set; }

    }
}
