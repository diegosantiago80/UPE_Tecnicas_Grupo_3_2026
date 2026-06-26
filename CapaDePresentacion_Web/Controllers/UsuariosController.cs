using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL;
using CapaDePresentacion_Web.Filters;
using System.Linq;

namespace CapaDePresentacion_Web.Controllers
{
    // gestion de usuarios: solo el administrador
    [SesionRequerida("Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();
        private readonly PerfilBLL _perfilBLL = BLLFactory.CrearPerfilBLL();

        public IActionResult Index()
        {
            var listaUsuarios = _usuarioBLL.ObtenerUsuarios();
            var listaOrdenada = listaUsuarios
                .Where(u => u.Activo)
                .OrderBy(u => AuthService.Instance.ObtenerPerfilPorId(u.IdPerfil)?.Nombre ?? "Sin Rol")
                .ThenBy(u => u.Nombre)
                .ThenBy(u => u.Apellido)
                .ToList();
            return View(listaOrdenada);
        }

        public IActionResult Crear()
        {
            ViewBag.Perfiles = _perfilBLL.ObtenerTodos();
            return View("Formulario", new Usuario { IdUsuario = 0, Activo = true });
        }

        public IActionResult Editar(int id)
        {
            var user = _usuarioBLL.ObtenerPorId(id);
            ViewBag.Perfiles = _perfilBLL.ObtenerTodos();
            return View("Formulario", user);
        }

        [HttpPost]
        public IActionResult Guardar(Usuario modelo)
        {
            try
            {
                int idLogueado = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

                if (modelo.IdUsuario > 0)
                {
                    _usuarioBLL.ActualizarUsuario(modelo, idLogueado);
                    TempData["Exito"] = "¡Usuario modificado exitosamente!";
                }
                else
                {
                    _usuarioBLL.AgregarUsuario(modelo, idLogueado);
                    TempData["Exito"] = "¡Usuario registrado exitosamente!";
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Perfiles = _perfilBLL.ObtenerTodos();
                return View("Formulario", modelo);
            }
        }

        public IActionResult Eliminar(int id)
        {
            try
            {
                int idLogueado = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

                _usuarioBLL.EliminarUsuario(id, idLogueado);
                TempData["Exito"] = "¡Usuario eliminado exitosamente!";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}