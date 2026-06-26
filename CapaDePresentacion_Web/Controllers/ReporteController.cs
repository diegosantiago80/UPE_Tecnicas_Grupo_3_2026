using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CapaDePresentacion_Web.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ReporteBLL_TemplateMethod _reportesBLL = new ReporteBLL_TemplateMethod();

        [HttpGet]
        [HttpPost]
        public IActionResult VerReporte(string periodo, string fechaFiltro, string buscarVendedor)
        {
            bool esPeticionPost = HttpContext.Request.Method == "POST";

            string vendedorLimpio = buscarVendedor?.Trim() ?? string.Empty;
            string fechaFinStr = periodo?.Trim() ?? string.Empty;
            string fechaInicioStr = fechaFiltro?.Trim() ?? string.Empty;

            // Si entra por primera vez
            if (!esPeticionPost && string.IsNullOrEmpty(fechaFinStr) && string.IsNullOrEmpty(fechaInicioStr) && string.IsNullOrEmpty(vendedorLimpio))
            {
                return View(new List<Reporte>());
            }

            DateTime hoy = DateTime.Today;
            DateTime? dInicio = null;
            DateTime? dFin = null;

            if (!string.IsNullOrEmpty(fechaInicioStr) && DateTime.TryParseExact(fechaInicioStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedInicio))
            {
                dInicio = parsedInicio;
            }

            if (!string.IsNullOrEmpty(fechaFinStr) && DateTime.TryParseExact(fechaFinStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedFin))
            {
                // SOLUCIÓN: Agregamos 23 horas, 59 mins y 59 segs para abarcar TODO el día
                dFin = parsedFin.Date.AddDays(1).AddTicks(-1);
            }

            // SOLUCIÓN DIARIA: Si solo llegó la fecha de inicio (búsqueda Diaria), 
            // convertimos dFin al último segundo de ese mismo día para que la BD encuentre las ventas.
            if (dInicio.HasValue && !dFin.HasValue)
            {
                dFin = dInicio.Value.Date.AddDays(1).AddTicks(-1);
            }

            if (esPeticionPost)
            {
                if (vendedorLimpio.Length > 50)
                {
                    ViewBag.ErrorMessage = "El nombre del vendedor no puede superar los 50 caracteres.";
                    return View(new List<Reporte>());
                }

                if (dInicio.HasValue && dInicio.Value.Date > hoy)
                {
                    ViewBag.ErrorMessage = "La fecha de inicio no puede ser una fecha futura.";
                    return View(new List<Reporte>());
                }

                if (dFin.HasValue && dFin.Value.Date > hoy)
                {
                    ViewBag.ErrorMessage = "La fecha de fin no puede ser una fecha futura.";
                    return View(new List<Reporte>());
                }

                if (dInicio.HasValue && dFin.HasValue && dInicio.Value.Date > dFin.Value.Date)
                {
                    ViewBag.ErrorMessage = "La fecha de inicio no puede ser mayor a la fecha de fin.";
                    return View(new List<Reporte>());
                }
            }

            try
            {
                // Ahora sí, las fechas viajan con el formato horario completo y preciso.
                var ventas = _reportesBLL.ObtenerVentasFiltradas(dInicio, dFin, vendedorLimpio);
                return View(ventas ?? new List<Reporte>());
            }
            catch (ArgumentException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<Reporte>());
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado al procesar el reporte de ventas. Intente nuevamente más tarde.";
                return View(new List<Reporte>());
            }
        }

        [HttpGet]
        public ActionResult GenerarEstadistica(string buscarMedicamento)
        {
            buscarMedicamento = buscarMedicamento ?? string.Empty;

            if (string.IsNullOrEmpty(buscarMedicamento))
                return View(new List<Reporte>());

            try
            {
                List<Reporte> lista = _reportesBLL.ObtenerEstadisticasMedicamento(buscarMedicamento);
                return View(lista ?? new List<Reporte>());
            }
            catch (ArgumentException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<Reporte>());
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Error al conectar con el servidor de estadísticas.";
                return View(new List<Reporte>());
            }
        }

        [HttpGet]
        public IActionResult Proyeccion()
        {
            return View(new List<Reporte>());
        }

        [HttpPost]
        public IActionResult Proyeccion(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            medicamento = medicamento?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(medicamento) && !fechaInicio.HasValue && !fechaFin.HasValue)
            {
                ViewBag.ErrorMessage = "Debe ingresar al menos un filtro para generar la proyección.";
                return View("Proyeccion", new List<Reporte>());
            }

            DateTime hoy = DateTime.Today;

            if (fechaInicio.HasValue && fechaInicio.Value.Date > hoy)
            {
                ViewBag.ErrorMessage = "La fecha de inicio no puede ser una fecha futura.";
                RetenerDatosFormularioProyeccion(medicamento, fechaInicio, fechaFin);
                return View("Proyeccion", new List<Reporte>());
            }

            if (fechaFin.HasValue)
            {
                // Parche de horario para proyecciones
                fechaFin = fechaFin.Value.Date.AddDays(1).AddTicks(-1);

                if (fechaFin.Value.Date > hoy)
                {
                    ViewBag.ErrorMessage = "La fecha de fin no puede ser una fecha futura.";
                    RetenerDatosFormularioProyeccion(medicamento, fechaInicio, fechaFin);
                    return View("Proyeccion", new List<Reporte>());
                }
            }

            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio.Value.Date > fechaFin.Value.Date)
            {
                ViewBag.ErrorMessage = "La fecha de inicio no puede ser mayor a la fecha de fin.";
                RetenerDatosFormularioProyeccion(medicamento, fechaInicio, fechaFin);
                return View("Proyeccion", new List<Reporte>());
            }

            try
            {
                var ventasProyectadas = _reportesBLL.FiltrarVentasComparativa(medicamento, fechaInicio, fechaFin);
                return View("Proyeccion", ventasProyectadas ?? new List<Reporte>());
            }
            catch (ArgumentException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                RetenerDatosFormularioProyeccion(medicamento, fechaInicio, fechaFin);
                return View("Proyeccion", new List<Reporte>());
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "No se pudo generar la proyección debido a un error interno.";
                return View("Proyeccion", new List<Reporte>());
            }
        }

        private void RetenerDatosFormularioProyeccion(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.MedicamentoBuscado = medicamento;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
        }
    }
}