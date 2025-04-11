
# Documentación Técnica del Sistema de Gestión de Negocio

Este documento contiene una descripción general de los componentes técnicos del sistema.

## Indice

1.[LoginController](#logincontroller)
   - [Login(UsuarioViewModel user)](#loginusuarioviewmodel-user)
   - [RecuperarClave(string correo)](#recuperarclavestring-correo)
   - [RestablecerClave(string token)](#restablecerclavestring-token)
   - [CerrarSesion()](#cerrarsesion)
2.[ProductoController](#productocontroller)
	-[AgregarProducto()](#agregarproducto())


---
## SE DEBE AGREGAR LA DOCUMENTACION DE 

-Configuration (para que sirve la clase de esa carpeta)
-EmailService (explicar la funcion)
-Como funciona recupearar y restablecer clave.

---

## LoginController

## Login(UsuarioViewModel user)

```csharp
        [HttpPost]
        public async Task<IActionResult> Login(UsuarioViewModel user)
        {
          
            var usuarioIngresado = context.Usuarios.FirstOrDefault(u => u.Correo == user.correo);
          
            if(usuarioIngresado==null || usuarioIngresado.Clave != ConvertirSha256(user.clave)) 
            {
                ModelState.AddModelError("correo", "Correo o contraseña incorrectas, intente de nuevo");
                return View();
            
            }
            else
            {
              
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,usuarioIngresado.Nombre),
                    new Claim("Correo",usuarioIngresado.Correo),
                    new Claim(ClaimTypes.Role,usuarioIngresado.Rol.ToString()),

                };

                var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));  

                return RedirectToAction("Index","Home");
            }
        }
```

---

- `var usuarioIngresado = context.Usuarios.FirstOrDefault(u => u.Correo == user.correo);`

	Buscar en la base de de datos si hay un registro con el mismo correo y lo guarda en `usuarioIngresado`.
---

```csharp
                  var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,usuarioIngresado.Nombre),
                    new Claim("Correo",usuarioIngresado.Correo),
                    new Claim(ClaimTypes.Role,usuarioIngresado.Rol.ToString()),

                };
```

   Creamos una lista de Claims. Los claims sirven para guardad algun dato relevante sobre el usuario. Al loguearse se crea una
   lista de claims que luego se guardaran en una Cookie.

---  
   `var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);`

   Reprensenta la identidad del usuario que se acaba de loguear . Anteriormente se creo una lista con todos los datos del usuario
   y en esta linea usamos esos datos para generar una Cookie que se guarda en el navegador del usuario que inicio sesion.

---
   `await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));`

   HttpContex es un objeto que contiene toda la informacion sobre la peticion HTTP actual. En este caso lo usamos para guardar
   los datos del usuario en una Cookie con `SignInAsync` y nos permite iniciar sesion.

---
## RecuperarClave(string correo)

```csharp
            [HttpPost]
        public async Task<IActionResult> RecuperarClave(string correo)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.Correo == correo);

            if(usuario==null) 
            {
                ModelState.AddModelError("correo", "No existe una cuenta vinculada a ese correo");
                return View();
            
            }

            string token= Guid.NewGuid().ToString();
            usuario.TokenConfirmacion=token;
            context.SaveChanges();

            string linkRecuperacion = Url.Action("RestablecerClave", "Login", new { token }, Request.Scheme);
            string mensaje = $"<p>Para restablecer tu contraseña, haz click <a href='{linkRecuperacion}'>aquí</a>.</p>";

            await emailService.EnviarCorreo(usuario.Correo, "Recuperacion de contraseña", mensaje);

            return RedirectToAction("CorreoEnviadoCambiarClave","Login");

        }
```

---
`var usuario = context.Usuarios.FirstOrDefault(u=> u.Correo == correo);`

Primero busca a un usuario con el correo y lo guarda en `usuario`.

```
      string token= Guid.NewGuid().ToString();
      usuario.TokenConfirmacion=token;
      context.SaveChanges();
```

Genera un guid que se utiliza como un token para el usuario que quiere cambiar la contraseña y se guarda ese token en la 
base de datos.

---
`string linkRecuperacion = Url.Action("RestablecerClave", "Login", new { token }, Request.Scheme);`

Se crea un link que apunta al metodo `RestablecerClave` del controlador `Login` y le envia por parametro el token.
`Request.Scheme` sirve para devolver el protocolo que se esta usando en la app (http o https), es importante este paso ya que 
sirve para generar la URL correcta para usarse fuera de la app, como en este caso que el metodo `RestablecerClave` se enviara
por mail.

---
`string mensaje = $"<p>Para restablecer tu contraseña, haz click <a href='{linkRecuperacion}'>aquí</a>.</p>";`

Se crea el mensaje que se quiere enviar por mail, el formato de los correos son en HTML.

---
`await emailService.EnviarCorreo(usuario.Correo, "Recuperacion de contraseña", mensaje);`

Se utiliza la funcion `EnviarCorreo` del service.

---
## RestablecerClave(string token)


```csharp

        public IActionResult RestablecerClave(string token)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion==token);
           
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
```
---
``` 
        public IActionResult RestablecerClave(string token)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion==token);
           
            if(usuario== null)
            {
                return NotFound();
            }

            return View(new RestablecerClaveViewModel { Token= token });
        }
```

Esta primera funcion lo que hace es recibir por parametro el token (es el que se envio con la Url en la funcion `RecuperarClave`).
Luego busca el token en la base de datos y si lo encuentra devuelve la vista con un viewModel con el token.

---
``` 
            usuario.Clave = ConvertirSha256(model.NuevaClave); //Bashea la contraseña
            usuario.TokenConfirmacion = null; //Elimina el token para que no se pueda vovler a acceder
            context.SaveChanges(); //Guarda los datos en la base de datos
```

En la segunda funcion se convierte la nueva clave en Bash, se elimina el token de confirmacion para que no se pueda volver a 
acceder y se guardan los datos en la base de datos.

---
## CerrarSesion()

```csharp
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Login");

        }
```

Elimina los datos de la Cookie con `SignOutAsync` y nos permite cerrar sesion.

---





	
   
## ProductoController

## AgregarProducto()
-	Explicacion de la funcion.

---







