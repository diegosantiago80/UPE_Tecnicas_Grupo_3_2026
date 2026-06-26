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

    public abstract class ProcesadorDeReportesTemplate
    {
        protected readonly IReporteDAL _reportesDal = new ReporteDALProxy();

        public List<Reporte> Procesar()
        {
            ValidarRequisitos();
            var ventasCrudas = ObtenerDatos();
            var ventasFiltradas = AplicarFiltros(ventasCrudas);
            return ventasFiltradas;
        }

        protected abstract void ValidarRequisitos();
        protected abstract List<Reporte> ObtenerDatos();
        protected abstract List<Reporte> AplicarFiltros(List<Reporte> ventas);
    }

    // ====================================================================================
    // IMPLEMENTACIONES CONCRETAS
    // ====================================================================================

    /// <summary>
    /// 2. PRIMERA SUBCLASE: Encargada del filtrado por Rango de Fechas.
    /// </summary>
    public class ProcesadorVentasPorPeriodo : ProcesadorDeReportesTemplate
    {
        private readonly DateTime? _fechaInicio;
        private readonly DateTime? _fechaFin;
        private readonly string _vendedor;

        public ProcesadorVentasPorPeriodo(DateTime? fechaInicio, DateTime? fechaFin, string vendedor)
        {
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
            _vendedor = vendedor?.Trim() ?? string.Empty;
        }

        protected override void ValidarRequisitos()
        {
            // Regla: Evitar filtros vacíos
            if (!_fechaInicio.HasValue && !_fechaFin.HasValue && string.IsNullOrWhiteSpace(_vendedor))
            {
                throw new ArgumentException("Debe seleccionar al menos un criterio de búsqueda (Fechas o Vendedor) para generar el reporte.");
            }

            // Regla: Longitud del nombre
            if (_vendedor.Length > 50)
            {
                throw new ArgumentException("El nombre del vendedor no puede exceder los 50 caracteres.");
            }

            DateTime hoy = DateTime.Today;

            // Regla: No permitir fechas futuras
            if (_fechaInicio.HasValue && _fechaInicio.Value.Date > hoy)
            {
                throw new ArgumentException("La fecha de inicio no puede ser una fecha futura.");
            }

            if (_fechaFin.HasValue && _fechaFin.Value.Date > hoy)
            {
                throw new ArgumentException("La fecha de fin no puede ser una fecha futura.");
            }

            // Regla: Coherencia de rangos
            if (_fechaInicio.HasValue && _fechaFin.HasValue)
            {
                if (_fechaInicio.Value.Date > _fechaFin.Value.Date)
                {
                    throw new ArgumentException("La 'Fecha Inicio' no puede ser mayor a la 'Fecha Fin'.");
                }
            }
        }

        protected override List<Reporte> ObtenerDatos()
        {
            // Enviamos _fechaInicio por si el SP de la BD quiere hacer un pre-filtro. 
            // El resto del filtrado detallado se hace en AplicarFiltros.
            return _reportesDal.ObtenerReporteVentas(_vendedor, null, _fechaInicio);
        }

        protected override List<Reporte> AplicarFiltros(List<Reporte> ventas)
        {
            if (ventas == null) return new List<Reporte>();

            // Filtrar por Nombre de Vendedor
            if (!string.IsNullOrEmpty(_vendedor))
            {
                ventas = ventas
                    .Where(v => v.NombreVendedor != null && v.NombreVendedor.ToLower().Contains(_vendedor.ToLower()))
                    .ToList();
            }

            // Filtrar por Fecha Única (Diaria) o Rango de Fechas (Semanal/Mensual)
            if (_fechaInicio.HasValue && !_fechaFin.HasValue)
            {
                ventas = ventas.Where(v => v.Fecha.Date == _fechaInicio.Value.Date).ToList();
            }
            else if (_fechaInicio.HasValue && _fechaFin.HasValue)
            {
                ventas = ventas.Where(v => v.Fecha.Date >= _fechaInicio.Value.Date && v.Fecha.Date <= _fechaFin.Value.Date).ToList();
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
            _medicamento = medicamento?.Trim() ?? string.Empty;
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
        }

        protected override void ValidarRequisitos()
        {
            if (string.IsNullOrWhiteSpace(_medicamento))
            {
                throw new ArgumentException("Debe ingresar el nombre del medicamento para realizar el análisis comparativo.");
            }

            DateTime hoy = DateTime.Today;

            if (_fechaInicio.HasValue && _fechaInicio.Value.Date > hoy)
                throw new ArgumentException("La fecha de inicio no puede ser futura.");

            if (_fechaFin.HasValue && _fechaFin.Value.Date > hoy)
                throw new ArgumentException("La fecha de fin no puede ser futura.");

            if (_fechaInicio.HasValue && _fechaFin.HasValue && _fechaFin.Value.Date < _fechaInicio.Value.Date)
            {
                throw new ArgumentException("La 'Fecha Fin' no puede ser anterior a la 'Fecha Inicio'.");
            }
        }

        protected override List<Reporte> ObtenerDatos()
        {
            return _reportesDal.ObtenerReporteVentas(string.Empty, null, null);
        }

        protected override List<Reporte> AplicarFiltros(List<Reporte> ventas)
        {
            if (ventas == null) return new List<Reporte>();

            var filtradas = ventas
                .Where(v => v.Medicamento != null && v.Medicamento.ToLower().Contains(_medicamento.ToLower()))
                .ToList();

            if (_fechaInicio.HasValue)
                filtradas = filtradas.Where(v => v.Fecha.Date >= _fechaInicio.Value.Date).ToList();

            if (_fechaFin.HasValue)
                filtradas = filtradas.Where(v => v.Fecha.Date <= _fechaFin.Value.Date).ToList();

            return filtradas;
        }
    }


    /// <summary>
    /// 4. TERCERA SUBCLASE: Estadisticas de medicamentos.
    /// </summary>
    public class ProcesadorEstadisticasMedicamento : ProcesadorDeReportesTemplate
    {
        private readonly string _medicamento;

        public ProcesadorEstadisticasMedicamento(string medicamento)
        {
            _medicamento = medicamento?.Trim() ?? string.Empty;
        }

        protected override void ValidarRequisitos() { }

        protected override List<Reporte> ObtenerDatos()
        {
            return _reportesDal.ObtenerEstadisticasTodosMedicamentos();
        }

        protected override List<Reporte> AplicarFiltros(List<Reporte> ventas)
        {
            if (ventas == null) return new List<Reporte>();
            if (string.IsNullOrEmpty(_medicamento)) return ventas;

            return ventas
                .Where(r => r.Medicamento != null && r.Medicamento.ToLower().Contains(_medicamento.ToLower()))
                .ToList();
        }
    }

    // ====================================================================================
    // USO DEL PATRÓN (Fachada de Negocio)
    // ====================================================================================
    public class ReporteBLL_TemplateMethod
    {
        // 🔹 NOTA: Se actualizó la firma para recibir dInicio y dFin en lugar de strings
        public List<Reporte> ObtenerVentasFiltradas(DateTime? fechaInicio, DateTime? fechaFin, string vendedor)
        {
            var procesador = new ProcesadorVentasPorPeriodo(fechaInicio, fechaFin, vendedor);
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