using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels
{
    public class InvitadoViewModel
    {
        [Required]
        public string? nombre {  get; set; }
        [Required]
        public string? nombreNegocio { get;set; }
    }
}
