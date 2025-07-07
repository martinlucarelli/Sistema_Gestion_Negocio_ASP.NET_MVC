namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Middleware
{


    //Este middleware intercepta cada request, y si el usuario está logueado (IsAuthenticated),
    //agrega headers que evitan que el navegador guarde la página en caché. Así, cuando presione "Atrás" después de
    //cerrar sesión, el navegador no podrá mostrar la página y pedirá una nueva  pero como ya no hay cookie, se lo redirige a
    //la vista pública.
    public class EliminarCacheMiddleware
    {

        private readonly RequestDelegate _next;

        public EliminarCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            
            if (context.User.Identity.IsAuthenticated)
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
            }

            await _next(context);
        }


    }
}
