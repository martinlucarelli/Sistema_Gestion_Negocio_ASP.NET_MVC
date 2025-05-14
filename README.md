# Sistema_Gestion_Negocio_ASP.NET_MVC



Aplicaci�n web desarrollada con ASP.NET Core MVC para gestionar ventas, productos y usuarios de distintos negocios. 
Dise�ada para ser utilizada por m�ltiples empresas con un �nico sistema, mediante roles y permisos.

---
## Indice

- [Tecnolog�as utilizadas](#tecnolog�as-utilizadas)
- [Arquitectura](#arquitectura)
- [Roles de usuario](#roles-de-usuario)
- [Funcionalidades principales](#funcionalidades-principales)
- [Base de datos](#base-de-datos)
- [Controllers](#controllers)
- [Services](#cervices)
- [Vistas Principales](#vistas-principales)
- [Notas de desarrollo](#notas-de-desarrollo)
- [En desarrollo](#en-desarrollo)
- [Autor](#autor)

## PROXIMO A HACER:

- Lo proximo a realizar es la vista que va mostrar todos los productos que perteneces a un negocio, para esto
voy a utilizar Jquery datatables que me va a permitir realizar la paginacion,ordenar columnas y buscar en tiempo
real.
- Para realizar la vista de productos se va a utilar una API , la api se va a crear dentro del mismo proyecto, y tambien
utilizara la carpeta controller, pero la logica va a estar dentro de la carpeta service, de esta manera aplicamos la inyeccin
de dependencias. Por lo tanto todo lo que este relacionado con los productos, tanto ver la lista o realizar operaciones, van
a pasar por la API interna del sistema, si combinamos la api con Fetch (JAVASCRIPT) nos va a permitir que cuando se realice 
alguna op�racion no va a ser necesario recargar la pagina para que se muestren esos cambios.





## Tecnolog�as utilizadas

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- C#
- Razor Pages
- Bootstrap 
- (Futuro) API REST para manejo din�mico del DOM

---

## Arquitectura

- Patr�n MVC (Modelo-Vista-Controlador)
- Autenticaci�n y autorizaci�n con roles
- Relaci�n multi-negocio (cada negocio tiene sus usuarios, productos y ventas)
- Generaci�n de PDFs y exportaci�n de datos a XLS

---

## Roles de usuario


|		Rol					|						Descripci�n 
|---------------------------|-------------------------------------------------------------------------|
| Administrador General		| Acceso completo a todo el sistema. Solo se crea desde la base de datos. |
|                           |                                                                         |
| Administrador de Negocio	| Due�o del negocio. Gestiona productos, ventas, empleados, etc.		  |
|                           |                                                                         |
| Empleado					| Acceso limitado. Puede registrar ventas, ver stock, etc.				  |
|                           |                                                                         |


---

## Funcionalidades principales

1. **Registrar nueva venta**
2. **Gesti�n de productos/stock**
3. **Historial de ventas**
   - Filtro por fechas
   - Detalle de venta (PDF)
   - Exportaci�n a XLS
4. **(Pr�ximamente) Estad�sticas con gr�ficos**

---

## Base de datos

- SQL Server
- Relaciones:
  - Un `Negocio` tiene muchos `Usuarios`, cada `Usuario` pertenece a un solo `Negocio`  
  - Un `Negocio` tiene muchos `Productos`, cada `Productos` pertenece a un solo `Negocio`
  - Las `Ventas` est�n relacionadas muchos-a-muchos con `Productos` mediante `DetalleVenta`

---

## Controllers
- **LoginController**  
	Encargado del inicio de sesion y restablecer contrase�a. Tambien tiene una funcion que se encarga de cifrar las contrase�as.
- **NegocioController**
	Encargado de mostrar los negocios que hay en la base de datos al administrador general. Ademas se encarga de enviar un correo
	a un cliente para poder realizar el registro de su usuario y de su negocio.

---

## Services
-**EmailService**
	Tiene la funcion que se encarga de enviar los mail para recuperar contrase�a o registrar usuario.

---

## Vistas principales

- **Login.cshtml**: Inicio de sesi�n.
- **RecuperarClave.cshtml**: Correo donde se va enviar el cambio de contrase�a
- **RestablecerClave.cshtml**: Formulario para cambiar la contrase�a.
- **Negocios**:Le muestran al Admin general los negocios del sistema.
- **AgregarNegocio.cshtml**:La usa el Admin general para mandar un mail a un cliente que contrato el servicio.
- **RegistrarAdministradorNegocio.cshtml**: Formulario para registrar al usuario due�o o administrador del negocio (se recibe por correo)
- **RegistrarNegocio**: Formulario para registrar negocio.

---

## Notas de desarrollo

- Se usa Guid para entidades principales y NanoId para otras tablas como Venta y Producto

- Los roles est�n definidos mediante un enum
- Autenticaci�n por correo y contrase�a
- El campo Confirmado arranca en false y se activa v�a token de email
- Las credenciales del correo se definen por dotnet user-secret para que no haya datos sensibles a la vista
- Rol, al ser un enum, se convierte con fluent api en el context para que EF lo tome como int
- Si el usuario esta 3 horas inactivo se le cierra la sesion automaticamente.
- Se utilizan Claims para guardar informacion del usuario en una Cookie.



## En dasarrollo

- [X] Login, confirmacion por correo y restablecer contrase�a por correo.
- [X] Autorizacion mediante roles del usuario, utilizando Cokies.
- [X] Form para que un cliente pueda completar los datos de su negocio (se envia por mail).
- [X] CRUD para que usuarios administradores de negocios registren usuarios empleados.
- [ ] Mostrar productos en seccion Stock.
- [ ] CRUD para productos (solo accesible para usuarios administradores de negocios).
- [ ] Mostrar lista de ventas, posibilidad de imprimir el detalle de una venta (tipo facutra).
- [ ] Mostrar lista de ventas especificando entre que fechas mostrar la lista, por defecto debe estar en el dia actual,pero se puede
cambiar las fechas con un calendario.
- [ ] Descargar en formato xls la tabla con las ventas.
- [ ] API REST para actualizar el DOM sin recargar la p�gina
- [ ] Estad�sticas con gr�ficos

## Autor

- Martin Lucarelli 
- Proyecto educativo

