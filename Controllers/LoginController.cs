using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    public class LoginController : Controller
    {

        NegocioContext context;
        public ILogger<LoginController> logger;
        private readonly EmailService emailService;
        private readonly BashClaveService bashClaveService;

        public LoginController(NegocioContext _context, ILogger<LoginController> _logger, EmailService _emailService, BashClaveService _bashClaveService)
        {
            context= _context;
            logger= _logger;
            emailService= _emailService;
            bashClaveService = _bashClaveService;

        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioViewModel user)
        {
          
            var usuarioIngresado = context.Usuarios.FirstOrDefault(u => u.Correo == user.correo);
          
            if(usuarioIngresado==null || usuarioIngresado.Clave != bashClaveService.ConvertirSha256(user.clave)) 
            {
                ModelState.AddModelError("correo", "Correo o contraseña incorrectas, intente de nuevo");
                return View();
            
            }
            else
            {
                //Al momento de iniciar sesion creamos las claims. Una claim es un dato que se guarda en la Cookie para identificar al usuario.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,usuarioIngresado.IdUsuario.ToString()),
                    new Claim("IdNegocio",usuarioIngresado.NegocioId.ToString()),
                    new Claim(ClaimTypes.Name,usuarioIngresado.Nombre),
                    new Claim("Correo",usuarioIngresado.Correo),
                    new Claim(ClaimTypes.Role,usuarioIngresado.Rol.ToString()),

                };

                var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));  

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
            usuario.Clave= bashClaveService.ConvertirSha256(model.ConfirmarClave);
            
            usuario.TokenConfirmacion = null; //Elimina el token para que no se pueda vovler a acceder
            context.SaveChanges(); //Guarda los datos en la base de datos

            return RedirectToAction("Login");
        }

        public IActionResult CorreoEnviadoCambiarClave()
        {
            return View();
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Login");

        }

    }

}
