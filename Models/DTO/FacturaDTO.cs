namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO
{
    public class FacturaDTO
    {
        public string? NombreNegocio {  get; set; }
        public string? DireccionNegocio { get; set; }
        public string? IdVenta {  get; set; }
        public DateTime fechaVenta { get; set; }
        public  string? FormaPago {  get; set; }
        public double Total {  get; set; }
        public string? Vendedor {  get; set; }

        public List<DetalleVentaDTO>? Detalle { get; set; } 


    }
}
