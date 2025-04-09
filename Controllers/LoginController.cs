using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;
using System.Security.Cryptography;
using System.Text;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    public class LoginController : Controller
    {

        NegocioContext context;
        public ILogger<LoginController> logger;
        private readonly EmailService emailService;

        public LoginController(NegocioContext _context, ILogger<LoginController> _logger, EmailService _emailService)
        {
            context= _context;
            logger= _logger;
            emailService= _emailService;

        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UsuarioViewModel user)
        {
          
            var usuarioIngresado = context.Usuarios.FirstOrDefault(u => u.Correo == user.correo);
          

            if(usuarioIngresado==null || usuarioIngresado.Clave != ConvertirSha256(user.clave)) 
            {
                ModelState.AddModelError("correo", "Correo o contraseña incorrectas, intente de nuevo");
                return View();
            
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }



        public IActionResult RecuperarClave()
        { 
            return View(); 
        }


        [HttpPost]
        public async Task<IActionResult> RecuperarClave(string correo)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.Correo == correo);

            if(usuario==null) 
            {
                ModelState.AddModelError("correo", "No existe una cuenta vinculada a ese correo");
                return View();
            
            }

            //Crea un token nuevo y lo guarda en la base de datos.
            string token= Guid.NewGuid().ToString();
            usuario.TokenConfirmacion=token;
            context.SaveChanges();

            //Crea el link para abrir la vista de reestablecer contraseña
            string linkRecuperacion = Url.Action("RestablecerClave", "Login", new { token }, Request.Scheme);
            string mensaje = $"<p>Para restablecer tu contraseña, haz click <a href='{linkRecuperacion}'>aquí</a>.</p>";

            await emailService.EnviarCorreo(usuario.Correo, "Recuperacion de contraseña", mensaje);

            return RedirectToAction("CorreoEnviadoCambiarClave","Login"); //Crear vista que avise que se envio un correo.

        }

        public IActionResult RestablecerClave(string token)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion==token);
            
            //Si no se encuentra el usuario con el token, devuelve un 404. Esto evita que alguien que no solicito cambio de contraseña lo haga
            if(usuario== null)
            {
                return NotFound();
            }

            return View(new RestablecerClaveViewModel { Token= token });
        }

        [HttpPost]
        public IActionResult RestablecerClave(RestablecerClaveViewModel model)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion == model.Token);

            if(usuario== null)
            {
                return NotFound();
            }

            if(model.NuevaClave != model.ConfirmarClave)
            {
                ModelState.AddModelError("ConfirmarContraseña", "Las contraseñas no coinciden");
                return View(model);

            }

            usuario.Clave = ConvertirSha256(model.NuevaClave); //Bashea la contraseña
            usuario.TokenConfirmacion = null; //Elimina el token para que no se pueda vovler a acceder
            context.SaveChanges(); //Guarda los datos en la base de datos

            return RedirectToAction("Login");
        }

        public IActionResult CorreoEnviadoCambiarClave()
        {
            return View();
        }















        //METODO PARA CIFRAR LAS CONTRASEÑAS
        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }

            }
            return Sb.ToString();
        }



    }

}
