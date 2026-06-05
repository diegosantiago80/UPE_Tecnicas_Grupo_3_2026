using Microsoft.AspNetCore.Mvc;
using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL;
using System.Linq;

namespace CapaDePresentacion_Web.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

        public IActionResult Index()
        {
            var listaUsuarios = _usuarioBLL.ObtenerUsuarios();

            var listaOrdenada = listaUsuarios
                .Where(u => u.Activo == true)
                .OrderBy(u => AuthService.Instance.ObtenerPerfilPorId(u.IdPerfil)?.Nombre ?? "Sin Rol")
                .ThenBy(u => u.Nombre)
                .ThenBy(u => u.Apellido)
                .ToList();

            return View(listaOrdenada);
        }

        public IActionResult Crear()
        {
            return View("Formulario", new Usuario { IdUsuario = 0, Activo = true });
        }

        public IActionResult Editar(int id)
        {
            var user = _usuarioBLL.ObtenerUsuarios().FirstOrDefault(u => u.IdUsuario == id);
            return View("Formulario", user);
        }

        [HttpPost]
        public IActionResult Guardar(Usuario modelo)
        {
            try
            {
                if (modelo.IdUsuario > 0)
                    _usuarioBLL.ActualizarUsuario(modelo);
                else
                    _usuarioBLL.AgregarUsuario(modelo);

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Formulario", modelo);
            }
        }

        public IActionResult Eliminar(int id)
        {
            _usuarioBLL.EliminarUsuario(id);
            return RedirectToAction("Index");
        }
    }
}
