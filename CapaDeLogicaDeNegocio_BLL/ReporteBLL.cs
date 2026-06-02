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

            // PASO 2 (Común): Obtener datos. (El proxy los entrega, ya sea de caché o del MockDAL)
            var ventasCrudas = _reportesDal.ObtenerTodasLasVentas();

            // PASO 3 (Específico): Aplicar filtros específicos en las subclases
            var ventasFiltradas = AplicarFiltros(ventasCrudas);

            return ventasFiltradas;
        }

        protected virtual bool ValidarRequisitos()
        {
            return true;
        }

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
        private readonly string _vendedor; // Añadido campo privado
        // Constructor actualizado para recibir el vendedor
        public ProcesadorVentasPorPeriodo(string periodo, DateTime? fechaFiltro, string vendedor)
        {
            _periodo = periodo;
            _fechaFiltro = fechaFiltro;
            _vendedor = vendedor;
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
    /// 4. TERCERA SUBCLASE (NUEVA): Encargada del cálculo y consolidado de estadísticas por medicamento.
    /// </summary>
    public class ProcesadorEstadisticasMedicamento : ProcesadorDeReportesTemplate
    {
        private readonly string _medicamento;

        public ProcesadorEstadisticasMedicamento(string medicamento)
        {
            _medicamento = medicamento;
        }

        protected override bool ValidarRequisitos()
        {
            return true; // No es obligatorio pasar un medicamento (traerá todos si está vacío)
        }

        protected override List<Reporte> AplicarFiltros(List<Reporte> ventas)
        {
            // 1. Filtrar las ventas crudas si se buscó un medicamento en particular
            var filtradas = ventas;
            if (!string.IsNullOrEmpty(_medicamento))
            {
                filtradas = ventas
                    .Where(v => v.Medicamento != null && v.Medicamento.ToLower().Contains(_medicamento.ToLower()))
                    .ToList();
            }

            // 2. Establecer rangos del mes actual e inicio/fin del mes anterior
            DateTime hoy = DateTime.Today;
            DateTime inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime inicioMesAnterior = inicioMesActual.AddMonths(-1);
            DateTime finMesAnterior = inicioMesActual.AddDays(-1);

            // 3. Agrupar por medicamento para calcular totales y variación mensual
            var reporteEstadisticas = filtradas
                .GroupBy(v => v.Medicamento)
                .Select(grupo =>
                {
                    string nombreMed = grupo.Key;
                    int unidadesTotales = grupo.Count();
                    int unidadesMesActual = grupo.Count(v => v.Fecha.Date >= inicioMesActual.Date && v.Fecha.Date <= hoy.Date);
                    int unidadesMesAnterior = grupo.Count(v => v.Fecha.Date >= inicioMesAnterior.Date && v.Fecha.Date <= finMesAnterior.Date);

                    decimal porcentajeVariacion = 0;
                    if (unidadesMesAnterior > 0)
                    {
                        porcentajeVariacion = ((decimal)(unidadesMesActual - unidadesMesAnterior) / unidadesMesAnterior) * 100;
                    }
                    else if (unidadesMesActual > 0)
                    {
                        porcentajeVariacion = 100; // Incremento del 100% si en el mes anterior no hubo ventas
                    }

                    // Se lee el stock disponible del primer registro del grupo en base a los datos mockeados en la DAL
                    int stock = grupo.FirstOrDefault()?.StockDisponible ?? 0;

                    return new Reporte
                    {
                        NombreMedicamento = nombreMed,
                        UnidadesVendidas = unidadesTotales,
                        PorcentajeVariacion = porcentajeVariacion,
                        StockDisponible = stock
                    };
                })
                .ToList();

            return reporteEstadisticas;
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