using Microsoft.AspNetCore.Mvc;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;
using System.Collections.Generic;

namespace CapaPresentacion_Web.Controllers
{
    public class LaboratorioController : Controller
    {
        // factory: creacion centralizada de servicios BLL
        private readonly LaboratorioBLL _laboratorioBLL = BLLFactory.CrearLaboratorioBLL();

        // CU-INV0004: listado de laboratorios
        public IActionResult Index()
        {
            var lista = _laboratorioBLL.ObtenerTodos();
            return View(lista);
        }

        // formulario de alta/edicion
        public IActionResult Gestionar(int id = 0)
        {
            var laboratorio = id > 0
                ? _laboratorioBLL.ObtenerTodos().Find(l => l.IdLaboratorio == id) ?? new Laboratorio()
                : new Laboratorio();
            return View(laboratorio);
        }

        [HttpPost]
        public IActionResult Guardar(Laboratorio laboratorio)
        {
            (bool exito, string mensaje) resultado;

            if (laboratorio.IdLaboratorio == 0)
                resultado = _laboratorioBLL.Agregar(laboratorio);
            else
                resultado = _laboratorioBLL.Modificar(laboratorio);

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
            var resultado = _laboratorioBLL.DarDeBaja(id);
            TempData[resultado.exito ? "Exito" : "Error"] = resultado.mensaje;
            return RedirectToAction("Index");
        }
    }
}
