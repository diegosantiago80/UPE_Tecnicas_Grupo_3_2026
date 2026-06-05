using System;
using System.Collections.Generic;
using System.Linq;
using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;

namespace CapaDeLogicaDeNegocio_BLL
{
    // ====================================================================================
    // PATRÓN DE DISEÑO: TEMPLATE METHOD (Método Plantilla)
    // ====================================================================================

    /// <summary>
    /// 1. CLASE BASE ABSTRACTA (El Esqueleto del Algoritmo)
    /// </summary>
    public abstract class ProcesadorDeReportesTemplate
    {
        // Conexión con el Proxy
        protected readonly IReporteDAL _reportesDal = new ReporteDALProxy();

        /// <summary>
        /// TEMPLATE METHOD (Método Plantilla)
        /// </summary>
        public List<Reporte> Procesar()
        {
            if (!ValidarRequisitos())
            {
                return new List<Reporte>();
            }

            // cada subclase sabe como obtener sus propios datos desde la DAL
            var ventasCrudas = ObtenerDatos();

            // aplicar filtros en memoria si la subclase lo necesita
            var ventasFiltradas = AplicarFiltros(ventasCrudas);

            return ventasFiltradas;
        }

        protected virtual bool ValidarRequisitos()
        {
            return true;
        }

        // cada subclase decide que metodo de la DAL llama
        protected abstract List<Reporte> ObtenerDatos();

        protected abstract List<Reporte> AplicarFiltros(List<Reporte> ventas);
    }


    // ====================================================================================
    // IMPLEMENTACIONES CONCRETAS (Las Clases Hijas)
    // ====================================================================================

    /// <summary>
    /// 2. PRIMERA SUBCLASE: Encargada del filtrado por Fecha y Periodo.
    /// </summary>
    public class ProcesadorVentasPorPeriodo : ProcesadorDeReportesTemplate
    {
        private readonly string _periodo;
        private readonly DateTime? _fechaFiltro;
        private readonly string _vendedor;

        public ProcesadorVentasPorPeriodo(string periodo, DateTime? fechaFiltro, string vendedor)
        {
            _periodo = periodo;
            _fechaFiltro = fechaFiltro;
            _vendedor = vendedor;
        }

        // llama al SP real con los parametros del reporte de ventas
        protected override List<Reporte> ObtenerDatos()
        {
            string nombreUsuario = string.IsNullOrEmpty(_vendedor) ? string.Empty : _vendedor;

            // si viene fecha puntual sin periodo, traemos todo y dejamos que AplicarFiltros
            // haga el filtro en memoria. el SP usa @Fecha solo para calcular rangos de periodo.
            string periodoParaSP = string.IsNullOrEmpty(_periodo) ? null : _periodo;
            DateTime? fechaParaSP = string.IsNullOrEmpty(_periodo) ? null : _fechaFiltro;

            return _reportesDal.ObtenerReporteVentas(nombreUsuario, periodoParaSP, fechaParaSP);
        }

        protected override List<Reporte> AplicarFiltros(List<Reporte> ventas)
        {
            // Regla de Negocio 0: Filtrar por Nombre de Vendedor si se especificó
            if (!string.IsNullOrEmpty(_vendedor))
            {
                ventas = ventas
                    .Where(v => v.NombreVendedor != null && v.NombreVendedor.ToLower().Contains(_vendedor.ToLower()))
                    .ToList();
            }
            // Regla de Negocio 1: Si hay una fecha exacta, filtramos por esa fecha.
            if (_fechaFiltro.HasValue)
            {
                ventas = ventas.Where(v => v.Fecha.Date == _fechaFiltro.Value.Date).ToList();
            }
            // Regla de Negocio 2: Si no hay fecha exacta pero sí un periodo (diaria, semanal...)
            else if (!string.IsNullOrEmpty(_periodo))
            {
                DateTime hoy = DateTime.Today;
                switch (_periodo.ToLower())
                {
                    case "diaria":
                        ventas = ventas.Where(v => v.Fecha.Date == hoy).ToList();
                        break;
                    case "semanal":
                        DateTime haceUnaSemana = hoy.AddDays(-7);
                        ventas = ventas.Where(v => v.Fecha.Date >= haceUnaSemana && v.Fecha.Date <= hoy).ToList();
                        break;
                    case "mensual":
                        ventas = ventas.Where(v => v.Fecha.Month == hoy.Month && v.Fecha.Year == hoy.Year).ToList();
                        break;
                }
            }
            return ventas;
        }
    }


    /// <summary>
    /// 3. SEGUNDA SUBCLASE: Encargada del filtrado Comparativo por Medicamento.
    /// </summary>
    public class ProcesadorVentasComparativa : ProcesadorDeReportesTemplate
    {
        private readonly string _medicamento;
        private readonly DateTime? _fechaInicio;
        private readonly DateTime? _fechaFin;

        public ProcesadorVentasComparativa(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            _medicamento = medicamento;
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
        }

        protected override bool ValidarRequisitos()
        {
            return !string.IsNullOrEmpty(_medicamento);
        }

        // trae todas las ventas sin filtro de vendedor para la proyeccion comparativa
        protected override List<Reporte> ObtenerDatos()
        {
            return _reportesDal.ObtenerReporteVentas(string.Empty, null, null);
        }

        protected override List<Reporte> AplicarFiltros(List<Reporte> ventas)
        {
            var filtradas = ventas.Where(v => v.Medicamento.ToLower().Contains(_medicamento.ToLower())).ToList();

            if (_fechaInicio.HasValue)
            {
                filtradas = filtradas.Where(v => v.Fecha.Date >= _fechaInicio.Value.Date).ToList();
            }

            if (_fechaFin.HasValue)
            {
                filtradas = filtradas.Where(v => v.Fecha.Date <= _fechaFin.Value.Date).ToList();
            }

            return filtradas;
        }
    }


    /// <summary>
    /// 4. TERCERA SUBCLASE: Estadisticas de medicamentos con stock real desde BD.
    /// </summary>
    public class ProcesadorEstadisticasMedicamento : ProcesadorDeReportesTemplate
    {
        private readonly string _medicamento;

        public ProcesadorEstadisticasMedicamento(string medicamento)
        {
            _medicamento = medicamento;
        }

        protected override bool ValidarRequisitos() => true;

        // usa el SP dedicado que devuelve unidades vendidas + stock real en una sola consulta
        protected override List<Reporte> ObtenerDatos()
        {
            return _reportesDal.ObtenerEstadisticasTodosMedicamentos();
        }

        // filtra por nombre si el gerente busco un medicamento especifico
        protected override List<Reporte> AplicarFiltros(List<Reporte> ventas)
        {
            if (string.IsNullOrEmpty(_medicamento))
                return ventas;

            return ventas
                .Where(r => r.Medicamento != null &&
                            r.Medicamento.ToLower().Contains(_medicamento.ToLower()))
                .ToList();
        }
    }


    // ====================================================================================
    // USO DEL PATRÓN (Fachada de Negocio)
    // ====================================================================================
    public class ReporteBLL_TemplateMethod
    {
        // Método actualizado
        public List<Reporte> ObtenerVentasFiltradas(string periodo, DateTime? fechaFiltro, string vendedor)
        {
            // Pasamos el vendedor al instanciar el procesador
            var procesador = new ProcesadorVentasPorPeriodo(periodo, fechaFiltro, vendedor);
            return procesador.Procesar();
        }
        public List<Reporte> FiltrarVentasComparativa(string medicamento, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var procesador = new ProcesadorVentasComparativa(medicamento, fechaInicio, fechaFin);
            return procesador.Procesar();
        }
        public List<Reporte> ObtenerEstadisticasMedicamento(string medicamento)
        {
            var procesador = new ProcesadorEstadisticasMedicamento(medicamento);
            return procesador.Procesar();
        }
    }
}