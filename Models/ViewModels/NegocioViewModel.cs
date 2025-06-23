using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels
{
    public class NegocioViewModel
    {
        public string nombre { get; set; }
        public string direccion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rubro válido.")]
        public int rubroId { get; set; }

        public string? tokenUsuario {  get; set; } //Campo utilizado para enviar token con la vista
    
        [NotMapped]
        public List<Rubro>? rubros { get; set; }

        
    }
}
