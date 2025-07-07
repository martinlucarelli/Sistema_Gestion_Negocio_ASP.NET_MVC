using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;
using System.Security.Claims;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    public class InvitadoController : Controller
    {

        NegocioContext context;
        public ILogger<InvitadoController> logger;
        
        public InvitadoController(NegocioContext _context, ILogger<InvitadoController> _logger)
        {
            context = _context;
            logger = _logger;

        }



        public IActionResult Inicio()
        {
            return View();
        }

        public IActionResult RegistrarInvitado()
        { 
            return View(); 
        }

        [HttpPost]
        public async Task< IActionResult> RegistrarInvitado(InvitadoViewModel invitado)
        {
            var negocio = new Negocio
            {
                Nombre = invitado.nombreNegocio,
                Direccion = "Av La Plata 9422",
                RubroId = 1001
            };
            context.Negocios.Add(negocio);
            var usuario = new Usuario
            {
                Correo = "userInvitado5719@gmail.com",
                Clave = "lfmn3ea90",
                Nombre = invitado.nombre,
                Confirmado = true,
                Rol = Rol.Invitado,
                NegocioId = negocio.IdNegocio

            };
            context.Usuarios.Add(usuario);

            await context.SaveChangesAsync();

            //AGREGAR 5 PRODUCTOS

            List<Producto> productos = new List<Producto>()
            {
                new Producto { Nombre = "Motherboard MSI B450M PRO-VDH", Precio = 120000, Stock = 5, NegocioId = negocio.IdNegocio },
                new Producto { Nombre = "Disco SSD Kingston 480GB", Precio = 45000, Stock = 20, NegocioId = negocio.IdNegocio },
                new Producto { Nombre = "Webcam Logitech C920", Precio = 22000, Stock = 12, NegocioId = negocio.IdNegocio },
                new Producto { Nombre = "Router TP-Link Archer C6", Precio = 14000, Stock = 40, NegocioId = negocio.IdNegocio },
                new Producto { Nombre = "Auriculares HyperX Cloud Stinger", Precio = 60000, Stock = 30, NegocioId = negocio.IdNegocio }


            };

            context.Productos.AddRange(productos);

            List<Usuario> usuarios = new List<Usuario>()
            {

                new Usuario { Correo = "empleado1@gmail.com", Clave = "123", Nombre = "Esteban", Confirmado = true, Rol = Rol.Empleado, NegocioId = negocio.IdNegocio },
                new Usuario { Correo = "empleado2@gmail.com", Clave = "123", Nombre = "Marcos", Confirmado = true, Rol = Rol.Empleado, NegocioId = negocio.IdNegocio },
                new Usuario { Correo = "empleado3@gmail.com", Clave = "123", Nombre = "Maria", Confirmado = true, Rol = Rol.Empleado, NegocioId = negocio.IdNegocio }

            };

            context.Usuarios.AddRange(usuarios);

            await context.SaveChangesAsync();
           

            
            
            
            
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,usuario.IdUsuario.ToString()),
                    new Claim("IdNegocio",usuario.NegocioId.ToString()),
                    new Claim(ClaimTypes.Name,usuario.Nombre),
                    new Claim("Correo",usuario.Correo),
                    new Claim(ClaimTypes.Role,usuario.Rol.ToString()),

                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("RegistrarVenta", "Venta");
 
        
        }
    }
}
