using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;
using CapaDePresentacion_Web.Filters;
using System.Collections.Generic;

namespace CapaPresentacion_Web.Controllers
{
    // stock y medicamentos: solo el encargado
    [SesionRequerida("Encargado")]
    public class MedicamentoController : Controller
    {
        // factory: creacion centralizada de servicios BLL
        private readonly MedicamentoBLL _medicamentoBLL = BLLFactory.CrearMedicamentoBLL();
        private readonly LaboratorioBLL _laboratorioBLL = BLLFactory.CrearLaboratorioBLL();
        private readonly CategoriaBLL _categoriaBLL  = BLLFactory.CrearCategoriaBLL();

        public IActionResult Index(bool soloCriticos = false)
        {
            List<Medicamento> lista;

            if (soloCriticos)
            {
                lista = _medicamentoBLL.ObtenerCriticos();
                ViewData["Titulo"] = "Alertas de Stock Crítico";

                // patron observer: las alertas se generaron al llamar ObtenerCriticos()
                // las pasamos a la vista para mostrarlas
                ViewBag.AlertasStock = AlertaStockObservador.AlertasActivas;
            }
            else
            {
                // solo activos — los dados de baja quedan en historicos de compras/ventas
                lista = _medicamentoBLL.ObtenerActivos();
                ViewData["Titulo"] = "Panel de Control de Stock";
            }

            return View(lista);
        }

        // CU-INV0003: formulario de alta/edicion
        public IActionResult Gestionar(int id = 0)
        {
            CargarCombos();
            var medicamento = id > 0 ? _medicamentoBLL.ObtenerPorId(id) ?? new Medicamento() : new Medicamento();
            return View(medicamento);
        }

        [HttpPost]
        public IActionResult Guardar(Medicamento medicamento)
        {
            (bool exito, string mensaje) resultado;

            int idLogueado = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            if (medicamento.IdMedicamento == 0)
                resultado = _medicamentoBLL.Agregar(medicamento, idLogueado);
            else
                resultado = _medicamentoBLL.Modificar(medicamento, idLogueado);

            if (resultado.exito)
            {
                TempData["Exito"] = resultado.mensaje;
                return RedirectToAction("Index");
            }

            TempData["Error"] = resultado.mensaje;
            CargarCombos();
            return View("Gestionar", medicamento);
        }

        [HttpPost]
        public IActionResult DarDeBaja(int id)
        {
            int idLogueado = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var resultado = _medicamentoBLL.DarDeBaja(id, idLogueado);
            TempData[resultado.exito ? "Exito" : "Error"] = resultado.mensaje;
            return RedirectToAction("Index");
        }

        // carga los combos de laboratorio y categoria para el formulario
        private void CargarCombos()
        {
            ViewBag.Laboratorios = _laboratorioBLL.ObtenerActivos();
            ViewBag.Categorias   = _categoriaBLL.ObtenerTodos();
        }
    }
}