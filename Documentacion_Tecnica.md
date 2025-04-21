
# Documentación Técnica del Sistema de Gestión de Negocio

Este documento contiene una descripción general de los componentes técnicos del sistema.

## Indice

1.[LoginController](#logincontroller)
   - [Login(UsuarioViewModel user)](#loginusuarioviewmodel-user)
   - [RecuperarClave(string correo)](#recuperarclavestring-correo)
   - [RestablecerClave(string token)](#restablecerclavestring-token)
   - [CerrarSesion()](#cerrarsesion)
2.[NegocioController](#negociocontroller)
   - [Negocios()](#negocios())
   - [AgregarNegocio(string correo)](#agregarnegocio(string-correo))
   - [RegistrarAdministradorNegocio(string token)](#registraradministradornegocio(string-token))
   - [RegistrarAdministradorNegocio(UsuarioViewModel user,string token)](#registraradministradoraegocio(usuarioviewModel-user,string-token))
   - [RegistrarNegocio(string token)](#registrarnegocio(string-token))
   - [RegistrarNegocio(NegocioViewModel n ,string token)](#registrarnegocio(negocioviewmodel-n,string-token))


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


## NegocioController

## Negocios()

Esta funcion devuelve la vista de los negocios que estan guardados en la base de datos

```csharp 
            var negocioConRubro = context.Negocios.Join(context.Rubros,
                negocio => negocio.RubroId,
                rubro => rubro.IdRubro,
                (negocio, rubro) => new
                {
                    negocio.IdNegocio,
                    negocio.Nombre,
                    negocio.Direccion,
                    rubroNombre = rubro.Nombre

                })
                .ToList();
               
            return View(negocioConRubro);
```

Se utiliza LINQ para realizar una consulta en la que creamos una nueva variable que la llamamos `negocioConRubro` y en ella
se va a guardar el resultado de la consulta. La consulta que utilizamos es un inner join para que en la vista en lugar de que
aparezca el id del rubro, aparezca el nombre del mismo.
Luego devolvemos la vista con la lista de los negocios con los rubros con nombres en lugar del id.
   
---

## AgregarNegocio(string correo)

Esta funcion asincronica es utilizada desde el rol del administrador general y sirve para enviar un correo a un cliente que
haya contratado el servicio, en ese correo estaran todos los formularios para registrar su negocio y su usuario.

``` csharp
public async Task<IActionResult> AgregarNegocio(string correo)
        {
            string token = Guid.NewGuid().ToString();

            logger.LogWarning($"El token que se genero fue : {token}"); //Log para ver el token en consola
            
            Usuario usuarioBandera = new Usuario()
            {
                Correo = correo,
                Clave = "Afjsirm11K22masl1nnde",
                Nombre = "a",
                Confirmado = false,
                TokenConfirmacion = token,
                Rol = Rol.AdministradorNegocio,
                NegocioId = Guid.Parse("06FDE092-6A5A-4156-B9A0-6327D8BB2FAD")
            };
            context.Usuarios.Add(usuarioBandera);
            context.SaveChanges();

            //Enlace para enviar correo de registro
            string linkRegistrarNegocio = Url.Action("RegistrarAdministradorNegocio", "Negocio", new { token }, Request.Scheme);
            string mensaje = $"<p>Para comenzar a registrar su negocio, haz click <a href='{linkRegistrarNegocio}'>aquí</a>.</p>";

            await emailService.EnviarCorreo(usuarioBandera.Correo, "Registrar negocio", mensaje);

            return View();
        }
```
---

`Usuario usuarioBandera = new Usuario()`

El usuario bandera sirve debido a que al momento de recibir el correo, el usuario debe tener un token que valide el acceso a ese
link. Por eso se crea un usuario bandera donde solamente se le asigna el correo y un token que generamos anteriormente. Los demas
datos son genericos y luego seran asignados los reales en la siguiente funcion.
En cuanto al Id del negocio al que pertenece este usuario bandera: Al momento de crear este usuario todavia no existe el negocio
al que va a pertenecer  , pero EF no nos permite asignar un Id de un negocio que no existe como clave foreanea y mucho menos
podemos dejarlo null, por lo tanto lo se hace es crear un negocio que esta en la base de datos y que solo sirve para almacenar
estos usuarios momentaneamente hasta que se cree el negocio real al que van a pertenecer y se les pueda asignar.
Por otro lado, se le asigna una clave sin bashear, por lo tanto es imposible que alguien acceda desde el login con ese usuario.

---

## RegistrarAdministradorNegocio(string token)

Esta funcion se encarga de devolver la vista donde se encuentra el formulario para registrar al usuario administrador del negocio
antes de devolver la vista corrobora si el token que tiene ese usuario es valido para darle acceso a la vista, evitando que
alguien sin permiso acceda a ella.

``` 

        [HttpGet]
        public IActionResult RegistrarAdministradorNegocio(string token)
        {
            logger.LogWarning($"El token que se recibio en RegistrarAdministradorNegocio fue : {token}");

            var usuario = context.Usuarios.FirstOrDefault(u => u.TokenConfirmacion == token);


            if (usuario == null) 
            {
                logger.LogError($"No se encontro ningun usuario con ese token en la base de datos");
                return NotFound(); 
            
            } //Se puede agregar una vista que diga que el link ya fue usado.

            return View(new UsuarioViewModel {token=token});
        }
```

Ademas se incluye el token del UsuarioViewModel para mantenerlo en el post:

``` cshtml

    <form method="post" asp-action="RegistrarAdministradorNegocio" asp-controller="Negocio">
        <input type="hidden" name="token" value="@Model.token" />
        ...
    </form>

```
---

## RegistrarAdministradorNegocio(UsuarioViewModel user,string token)

Esta funcion es el post de la anterior y contiene la logica para terminar de guardar los datos en el usuario bandera que se creo
anteriormente en la funcion `agregarNegocio()`

``` 
        public  IActionResult RegistrarAdministradorNegocio(UsuarioViewModel user,string token)
        {
            var usuarioActualizar =context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion == token);
            if(usuarioActualizar == null)
            {
                ModelState.AddModelError("correo", "no encotramos un usuario con esas");
                return View(user);
            }

            if (context.Usuarios.Any(u => u.Correo == user.correo))
            {
                ModelState.AddModelError("correo", "Ya existe un usuario registrado con ese correo");
                return View(user);
            }

            if (user.clave == user.repetirClave)
            {
                string claveCifrada= bashClaveService.ConvertirSha256(user.clave);
                
                usuarioActualizar.Correo = user.correo;
                usuarioActualizar.Nombre = user.nombre;
                usuarioActualizar.Clave = claveCifrada;
                usuarioActualizar.Rol = Rol.AdministradorNegocio;
  
                context.Usuarios.Update(usuarioActualizar);
                context.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("clave", "Las claves no coinciden");
                return View(user);
            }
            return RedirectToAction("AdminNegocioRegistradoConExito", "Negocio", new {token});
        }
```

Despues de pasar los condicionales para busacar el usuario con el token, verificar que no se exista esa direccion de correo en 
el sistema y verificar que no se repitan las claves, se pasa a hacer un update del usuario bandera ya guardando sus datos
reales, los unicos datos que todavia no se asignarion fueron:

`IdNegocio`: Todavia no esta creado el negocio, asi que sigue con el id del negocio del sistema que sirve para estos casos-
`Confirmado`: Como no pertenece a ningun negocio real, todavia no se confirma este usuario, esto evita que inicie sesion.

---

## RegistrarNegocio(string token)

Para acceder a esta funcion se hace desde el enlace de la vista `AdminNegocioRegistradoConExito`, en esta vista enviamos
el token en la URL

```cshtml

<a asp-action="RegistrarNegocio" asp-controller="Negocio" asp-route-token="@Model.token">
    Registrar negocio
</a>
```

Esta funcion devuelve la vista para registar al negocio y con ella el view model de negocio. Dentro del view model esta la 
lista de rubros y ademas el token del usuario. Si bien el token no es un atributo directo de negocio, ya que el negocio no tiene
token, debi ponerlo en el viewmodel ya que es necesario para enviar por parametro.

``` 
        [HttpGet]
        public IActionResult RegistrarNegocio(string token)
        {
            var usuario = context.Usuarios.FirstOrDefault(u => u.TokenConfirmacion == token);
            if (usuario == null) return NotFound();

            var modelo = new NegocioViewModel
            {
                rubros = context.Rubros.ToList(),
                tokenUsuario = token
            };

            return View(modelo);
        }
```

En la vista de esta funcion se incuye el token en modo hidden

`<input type="hidden" name="token" value="@Model.tokenUsuario" />`


---

## RegistrarNegocio(NegocioViewModel n ,string token)

Esta funcion finalmente es la encargada de registrar un negocio y guardarlo en la base de datos y luego, con el token que
recibe por parametro, busca al usuario que se guardo como "bandera" anteriormente y le asigna el id del nuevo negocio que
ya se creo y ademas lo marca como usuario confirmado. Aqui ya finaliza el registro de un dueño de negocio y su negocio.

``` 
        [HttpPost]
        public IActionResult RegistrarNegocio(NegocioViewModel n ,string token)
        {
            Negocio negocioFinal = new Negocio()
            {
                Nombre = n.nombre,
                Direccion = n.direccion,
                RubroId = n.rubroId
            };
            context.Negocios.Add(negocioFinal);
            context.SaveChanges();

            Usuario usuarioActualizar = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion == token);
            
            if(usuarioActualizar == null)
            {
                return NotFound();
            }
            else 
            {
                usuarioActualizar.NegocioId = negocioFinal.IdNegocio;
                usuarioActualizar.TokenConfirmacion = null;
                usuarioActualizar.Confirmado = true;
                context.Usuarios.Update(usuarioActualizar);
                context.SaveChanges();
            }

            return RedirectToAction("NegocioRegistradoConExito", "Negocio");
        }

```









