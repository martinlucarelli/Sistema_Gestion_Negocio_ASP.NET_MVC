using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels
{
    public class InvitadoViewModel
    {
        [Required (ErrorMessage ="Debes ingresar tu nombre")]
        public string? nombre {  get; set; }
        [Required(ErrorMessage = "Debes ingresar el nombre de tu negocio")]
        public string? nombreNegocio { get;set; }
    }
}
