using Microsoft.AspNetCore.Mvc;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;
using CapaDePresentacion_Web.Filters;
using System.Collections.Generic;

namespace CapaDePresentacion_Web.Controllers
{
    // perfiles y permisos: solo el administrador
    [SesionRequerida("Administrador")]
    public class PerfilesController : Controller
    {
        private readonly PerfilBLL _perfilBLL = BLLFactory.CrearPerfilBLL();

        public IActionResult Index()
        {
            var lista = _perfilBLL.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Gestionar(int id = 0)
        {
            var perfil = id > 0 ? _perfilBLL.ObtenerPorId(id) ?? new Perfil() : new Perfil();
            return View(perfil);
        }

        [HttpPost]
        public IActionResult Guardar(Perfil perfil, List<string> permisosSeleccionados)
        {
            perfil.PermisosAsignados = permisosSeleccionados != null && permisosSeleccionados.Count > 0
                ? string.Join(",", permisosSeleccionados)
                : "";

            int idLogueado = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            var resultado = _perfilBLL.Guardar(perfil, idLogueado);

            if (resultado.exito)
            {
                TempData["Exito"] = resultado.mensaje;
                return RedirectToAction("Index");
            }

            TempData["Error"] = resultado.mensaje;
            return View("Gestionar", perfil);
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            int idLogueado = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            var resultado = _perfilBLL.Eliminar(id, idLogueado);
            TempData[resultado.exito ? "Exito" : "Error"] = resultado.mensaje;
            return RedirectToAction("Index");
        }
    }
}