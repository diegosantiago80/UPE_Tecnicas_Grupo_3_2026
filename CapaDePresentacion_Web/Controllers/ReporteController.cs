using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CapaDePresentacion_Web.Controllers
{
    // NO PONER ACÁ NINGÚN [Route(...)]. El framework usa la ruta convencional: /Reporte
    public class ReporteController : Controller
    {
        private readonly ReporteBLL _reportesBLL = new ReporteBLL();

        // URL: /Reporte/VerReporte
        public IActionResult VerReporte(string periodo, DateTime? fechaFiltro)
        {
            var ventas = _reportesBLL.ObtenerVentasFiltradas(periodo, fechaFiltro);
            return View(ventas);
        }

        [HttpGet]
        public IActionResult GenerarEstadistica()
        {
            return View("GenerarEstadistica");
        }

        // Ruta: localhost:XXXX/Reporte/Proyeccion
        [HttpGet]
        public IActionResult Proyeccion()
        {
            return View("Proyeccion");
        }

        // La ruta para acceder a esta página será: /Comparativa/Medicamentos
        [HttpGet("Comparativa/Medicamentos")]
        public IActionResult Medicamentos(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            // 2. Le delegamos TODO el trabajo pesado a la BLL. 
            // El controlador ya no tiene que hacer "Ifs", ni "Wheres", ni filtrar nada.
            // Solo le pasa los parámetros que llegaron por la URL y la BLL se encarga del resto.
            var ventasFiltradas = _reportesBLL.FiltrarVentasComparativa(medicamento, fechaInicio, fechaFin);

            // 3. Devolvemos la vista enviando la lista ya procesada
            return View("ComparativaMedicamentos", ventasFiltradas);
        }
    }
}