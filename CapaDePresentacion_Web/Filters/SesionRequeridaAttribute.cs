using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CapaDePresentacion_Web.Filters
{
    // filtro reutilizable para proteger actions que requieren sesion activa
    // opcionalmente acepta uno o mas roles permitidos
    // uso: [SesionRequerida] o [SesionRequerida("Administrador", "Gerente")]
    public class SesionRequeridaAttribute : ActionFilterAttribute
    {
        private readonly string[] _rolesPermitidos;

        public SesionRequeridaAttribute(params string[] rolesPermitidos)
        {
            _rolesPermitidos = rolesPermitidos;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            int? idUsuario = session.GetInt32("IdUsuario");

            // si no hay sesion activa, redirigir al login
            if (idUsuario == null || idUsuario == 0)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // si se especificaron roles, verificar que el usuario tenga uno de ellos
            if (_rolesPermitidos.Length > 0)
            {
                string rolActual = session.GetString("NombreRol") ?? string.Empty;
                bool tienePermiso = Array.Exists(_rolesPermitidos, r =>
                    string.Equals(r, rolActual, StringComparison.OrdinalIgnoreCase));

                if (!tienePermiso)
                {
                    // usuario logueado pero sin permiso: redirigir a acceso denegado
                    context.Result = new RedirectToActionResult("AccesoDenegado", "Account", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
