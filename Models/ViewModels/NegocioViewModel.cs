using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels
{
    public class NegocioViewModel
    {
        public string nombre { get; set; }
        public string direccion { get; set; }
        public int rubroId { get; set; }

        public string tokenUsuario {  get; set; } //Campo utilizado para enviar token con la vista
    
        [NotMapped]
        public List<Rubro> rubros { get; set; }
    }
}
