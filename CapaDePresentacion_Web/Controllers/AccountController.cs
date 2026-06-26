using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CapaDeLogicaDeNegocio_BLL;

namespace CapaDePresentacion_Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string user, string pass)
        {
            var usuarioValido = AuthService.Instance.ValidarUsuario(user, pass);

            if (usuarioValido != null)
            {
                HttpContext.Session.SetInt32("IdUsuario",    usuarioValido.IdUsuario);
                HttpContext.Session.SetInt32("IdPerfil",     usuarioValido.IdPerfil);
                HttpContext.Session.SetString("NombreUsuario", usuarioValido.NombreReal);
                HttpContext.Session.SetString("NombreRol",   usuarioValido.NombreRol);
                HttpContext.Session.SetString("DniUsuario",  usuarioValido.Dni);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // destino cuando un usuario logueado intenta acceder a una seccion sin permiso
        public IActionResult AccesoDenegado()
        {
            string rol = HttpContext.Session.GetString("NombreRol") ?? "desconocido";
            ViewBag.Rol = rol;
            return View();
        }
    }
}