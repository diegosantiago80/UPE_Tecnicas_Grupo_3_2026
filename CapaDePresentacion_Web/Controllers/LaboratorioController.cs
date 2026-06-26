using Microsoft.AspNetCore.Mvc;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;
using CapaDePresentacion_Web.Filters;

namespace CapaDePresentacion_Web.Controllers
{
    // laboratorios: administracion de proveedores, solo encargado
    [SesionRequerida("Encargado")]
    public class LaboratorioController : Controller
    {
        // factory: creacion centralizada de servicios BLL
        private readonly LaboratorioBLL _laboratorioBLL = BLLFactory.CrearLaboratorioBLL();

        // CU-INV0004: listado de laboratorios (solo activos — los inactivos quedan en historicos)
        public IActionResult Index()
        {
            var lista = _laboratorioBLL.ObtenerActivos();
            return View(lista);
        }

        // formulario de alta/edicion
        public IActionResult Gestionar(int id = 0)
        {
            var laboratorio = id > 0
                ? _laboratorioBLL.ObtenerPorId(id) ?? new Laboratorio()
                : new Laboratorio();
            return View(laboratorio);
        }

        [HttpPost]
        public IActionResult Guardar(Laboratorio laboratorio)
        {
            int idLogueado = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            (bool exito, string mensaje) resultado;

            if (laboratorio.IdLaboratorio == 0)
                resultado = _laboratorioBLL.Agregar(laboratorio, idLogueado);
            else
                resultado = _laboratorioBLL.Modificar(laboratorio, idLogueado);

            if (resultado.exito)
            {
                TempData["Exito"] = resultado.mensaje;
                return RedirectToAction("Index");
            }

            TempData["Error"] = resultado.mensaje;
            return View("Gestionar", laboratorio);
        }

        [HttpPost]
        public IActionResult DarDeBaja(int id)
        {
            int idLogueado = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            var resultado = _laboratorioBLL.DarDeBaja(id, idLogueado);
            TempData[resultado.exito ? "Exito" : "Error"] = resultado.mensaje;
            return RedirectToAction("Index");
        }
    }
}
