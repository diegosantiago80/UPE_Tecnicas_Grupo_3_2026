using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CapaDePresentacion_Web.Controllers
{
    // ====================================================================================
    // CAPA DE PRESENTACIÓN: El Controlador (Controller)
    // ====================================================================================
    // El Controlador es el puente entre lo que el usuario ve en la web (Vista) y las
    // reglas del negocio (BLL). NUNCA debe conectarse a la base de datos (DAL) ni 
    // realizar cálculos complejos. Su único trabajo es recibir peticiones HTTP, 
    // enviarlas a la BLL, y devolver la pantalla (Vista) adecuada con los resultados.
    // ====================================================================================

    // NO PONER ACÁ NINGÚN [Route(...)]. El framework MVC usa la ruta convencional: /Reporte
    public class ReporteController : Controller
    {
        // --------------------------------------------------------------------------------
        // 1. CONEXIÓN CON LA CAPA DE NEGOCIO (BLL)
        // --------------------------------------------------------------------------------
        // Aquí instanciamos la "Fachada" que creamos con el patrón Template Method.
        // El controlador NO sabe (ni le importa) que por detrás hay un "Template Method"
        // o un "Proxy de Caché". Solo sabe que al usar esta clase, obtendrá las ventas.
        // Esto se llama "Bajo Acoplamiento", una excelente práctica.
        private readonly ReporteBLL_TemplateMethod _reportesBLL = new ReporteBLL_TemplateMethod();


        // --------------------------------------------------------------------------------
        // 2. ACCIONES DEL CONTROLADOR (Endpoints)
        // --------------------------------------------------------------------------------

        // URL para acceder a este método en tu navegador: /Reporte/VerReporte
        public IActionResult VerReporte(string periodo, DateTime? fechaFiltro, string buscarVendedor)
        {
            // Evitamos posibles nulos en la búsqueda de texto
            buscarVendedor = buscarVendedor ?? string.Empty;
            periodo = periodo ?? string.Empty;

            // PASO 1: Recibimos los parámetros provenientes de la URL o formulario

            // PASO 2: Le delegamos el trabajo a la capa de Negocio (BLL).
            // Usamos la instancia global de la clase.
            var ventas = _reportesBLL.ObtenerVentasFiltradas(periodo, fechaFiltro, buscarVendedor);

            // PASO 3: Devolvemos la interfaz gráfica (Vista) enviándole la lista de ventas finales.
            return View(ventas ?? new List<Reporte>());
        }

        // URL: /Reporte/GenerarEstadistica
        [HttpGet]
        public ActionResult GenerarEstadistica(string buscarMedicamento)
        {
            // Evitamos posibles nulos en la cadena de búsqueda
            buscarMedicamento = buscarMedicamento ?? string.Empty;

            // Obtenemos los reportes procesados por el Template Method usando el atributo global
            List<Reporte> lista = _reportesBLL.ObtenerEstadisticasMedicamento(buscarMedicamento);

            // Devolverá automáticamente la vista "GenerarEstadistica.cshtml"
            return View(lista ?? new List<Reporte>());
        }

        // URL: /Reporte/Proyeccion (Carga inicial de la pantalla por GET)
        [HttpGet]
        public IActionResult Proyeccion()
        {
            // Carga la pantalla con una lista vacía hasta que el Gerente presione "Filtrar"
            return View(new List<Reporte>());
        }

        // URL: /Reporte/Proyeccion (Procesa el formulario cuando se presionan los filtros por POST)
        [HttpPost]
        public IActionResult Proyeccion(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            medicamento = medicamento ?? string.Empty;

            // PASO 1: Recibimos los datos de búsqueda.

            // PASO 2: Le delegamos TODO el trabajo a la BLL. 
            // Fíjate que el controlador está "limpio". No hay condicionales lógicos ("Ifs"), 
            // no hay filtros directos a listas ("Where"). Todo está centralizado en el Patrón
            // Template Method que diseñamos en la capa inferior.
            var ventasProyectadas = _reportesBLL.FiltrarVentasComparativa(medicamento, fechaInicio, fechaFin);

            // PASO 3: Devolvemos la vista visual de la proyección ("Proyeccion.cshtml"), 
            // inyectándole los datos que pasaron por el Template Method y el Proxy de Caché.
            return View("Proyeccion", ventasProyectadas ?? new List<Reporte>());
        }
    }
}