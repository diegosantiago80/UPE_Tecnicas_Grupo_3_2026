using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL;
using Microsoft.AspNetCore.Mvc;
using System;

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
            //instanciamos el servicio bll
            ReporteBLL_TemplateMethod objetoBll = new ReporteBLL_TemplateMethod();

            //Evitamos posibles nulo en la busqueda de texto
            buscarVendedor = buscarVendedor ?? "";
            periodo = periodo ?? "";

            // PASO 1: Recibimos los parámetros provenientes de la URL o formulario (ej. ?periodo=diaria)

            // PASO 2: Le delegamos el trabajo a la capa de Negocio (BLL).
            // Es la BLL la que se encarga de aplicar los filtros usando nuestro Template Method.
            var ventas = _reportesBLL.ObtenerVentasFiltradas(periodo, fechaFiltro,buscarVendedor);

            // PASO 3: Devolvemos la interfaz gráfica (Vista) enviándole la lista de ventas finales.
            return View(ventas);
        }

        // URL: /Reporte/GenerarEstadistica
        [HttpGet]
        // Acción para cargar la vista GenerarEstadistica.cshtml
        public ActionResult GenerarEstadistica(string buscarMedicamento)
        {
            // Instanciamos el servicio BLL
            ReporteBLL_TemplateMethod objetoBll = new ReporteBLL_TemplateMethod();
            // Evitamos posibles nulos en la cadena de búsqueda
            buscarMedicamento = buscarMedicamento ?? "";
            // Obtenemos los reportes procesados por el Template Method
            List<Reporte> lista = objetoBll.ObtenerEstadisticasMedicamento(buscarMedicamento);
            // Devolverá automáticamente la vista "GenerarEstadistica.cshtml"
            return View(lista);
        }

        // URL: /Reporte/Proyeccion
        [HttpGet]
        public IActionResult Proyeccion(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            // Reutilizamos la BLL para traer los datos filtrados para la proyección
            var ventasProyectadas = _reportesBLL.FiltrarVentasComparativa(medicamento, fechaInicio, fechaFin);
            // Devolvemos la vista de proyección enviándole los datos encontrados
            return View("Proyeccion", ventasProyectadas);
        }

         // La ruta específica para acceder a esta página cambia por el atributo: /Comparativa/Medicamentos
         [HttpGet("Comparativa/Medicamentos")]
        public IActionResult Medicamentos(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            // PASO 1: Recibimos los datos de búsqueda.

            // PASO 2: Le delegamos TODO el trabajo a la BLL. 
            // Fíjate que el controlador está "limpio". No hay condicionales lógicos ("Ifs"), 
            // no hay filtros directos a listas ("Where"). Todo está centralizado en el Patrón
            // Template Method que diseñamos en la capa inferior.
            var ventasFiltradas = _reportesBLL.FiltrarVentasComparativa(medicamento, fechaInicio, fechaFin);

            // PASO 3: Devolvemos la vista visual de la comparativa, inyectándole 
            // los datos que pasaron por el Template Method y el Proxy de Caché.
            return View("ComparativaMedicamentos", ventasFiltradas ?? new List<Reporte>());
        }
    }
}
