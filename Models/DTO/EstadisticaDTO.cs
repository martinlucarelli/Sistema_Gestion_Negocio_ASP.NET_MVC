namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO
{
    public class EstadisticaDTO
    {

        public List<ItemGrafico> VentasPorPeriodo { get; set; } 
        public List<ItemGrafico> GananciasPorPeriodo { get; set; }
        public List<ProductoEstadistica> ProductosMasVendidos { get; set; }
        public List<FormaPagoEstadistica> FormasDePago { get; set; }
    }


  
    public class ItemGrafico
    {
        public string Periodo { get; set; }
        public double Valor { get; set; } //cantidad de ventas o suma de ganancias
    }

    public class ProductoEstadistica
    {
        public string Nombre { get; set; }
        public int CantidadVendida { get; set; }
    }

    public class FormaPagoEstadistica
    {
        public string Metodo { get; set; }
        public int Cantidad { get; set; }
    }

    public enum Tiempo
    {
        ultimoDia = 100,
        ultimaSemana = 101,
        ultimoMes = 102,
        ultimos3Meses = 103,
        ultimos6Meses = 104,
        ultimoAnio = 105,

        Enero=1,
        Febrero=2,
        Marzo=3,
        Abril=4,
        Mayo=5,
        Junio=6,
        Julio=7,
        Agosto=8,
        Septiembre=9,
        Octubre=10,
        Noviembre=11,
        Diciembre=12
    }

}
