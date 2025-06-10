using System.Text.Json.Serialization;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models
{
    public class FormaPago
    {
        public int IdFormaPago { get; set; }
        public string TipoFormaPago { get; set; }

        //Relacion con venta
        [JsonIgnore]
        public ICollection<Venta> ventas { get; set; }
    }
}
