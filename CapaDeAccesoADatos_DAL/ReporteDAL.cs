using CapaDeEntidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaDeAccesoADatos_DAL
{
    // ====================================================================================
    // PATRÓN DE DISEÑO: PROXY (Intermediario o Sustituto)
    // ====================================================================================

    /// <summary>
    /// 1. INTERFAZ DEL SUJETO (ISubject)
    /// </summary>
    public interface IReporteDAL
    {
        List<Reporte> ObtenerTodasLasVentas();
    }


    /// <summary>
    /// 2. OBJETO REAL (RealSubject) con Simulación de Base de Datos
    /// </summary>
    public class ReporteDALReal : IReporteDAL
    {
        public List<Reporte> ObtenerTodasLasVentas()
        {
            // Simulamos el costo de ir a la Base de Datos
            Console.WriteLine("[ReporteDALReal] Ejecutando: Conectando a SQL Server y obteniendo datos reales...");

            // Obtenemos fechas de referencia relativas al día de hoy para que los filtros temporales siempre den resultados
            DateTime hoy = DateTime.Today;
            DateTime inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime inicioMesAnterior = inicioMesActual.AddMonths(-1);

            return new List<Reporte>
            {
                // ==========================================================================
                // 1. PARACETAMOL: Stock 45, Ventas Mes Actual: 5, Ventas Mes Anterior: 3 (+66% variación)
                // ==========================================================================
                new Reporte { NombreVendedor = "Carlos Perez", Fecha = hoy.AddDays(-1), Hora = TimeSpan.Parse("10:30"), Monto = 1200.00m, Medicamento = "Paracetamol", StockDisponible = 45 },
                new Reporte { NombreVendedor = "Ana Gomez", Fecha = hoy.AddDays(-3), Hora = TimeSpan.Parse("14:15"), Monto = 1800.00m, Medicamento = "Paracetamol", StockDisponible = 45 },
                new Reporte { NombreVendedor = "Luis Martinez", Fecha = hoy.AddDays(-5), Hora = TimeSpan.Parse("09:45"), Monto = 900.00m, Medicamento = "Paracetamol", StockDisponible = 45 },
                new Reporte { NombreVendedor = "Carlos Perez", Fecha = inicioMesActual.AddDays(1), Hora = TimeSpan.Parse("16:20"), Monto = 2100.00m, Medicamento = "Paracetamol", StockDisponible = 45 },
                new Reporte { NombreVendedor = "Sofia Castro", Fecha = inicioMesActual.AddDays(3), Hora = TimeSpan.Parse("11:10"), Monto = 1800.00m, Medicamento = "Paracetamol", StockDisponible = 45 },

                new Reporte { NombreVendedor = "Ana Gomez", Fecha = inicioMesAnterior.AddDays(5), Hora = TimeSpan.Parse("10:00"), Monto = 1200.00m, Medicamento = "Paracetamol", StockDisponible = 45 },
                new Reporte { NombreVendedor = "Sofia Castro", Fecha = inicioMesAnterior.AddDays(12), Hora = TimeSpan.Parse("11:30"), Monto = 1200.00m, Medicamento = "Paracetamol", StockDisponible = 45 },
                new Reporte { NombreVendedor = "Carlos Perez", Fecha = inicioMesAnterior.AddDays(20), Hora = TimeSpan.Parse("16:45"), Monto = 1200.00m, Medicamento = "Paracetamol", StockDisponible = 45 },

                // ==========================================================================
                // 2. IBUPROFENO: Stock 8 (Nivel Crítico), Ventas Mes Actual: 2, Ventas Mes Anterior: 4 (-50% variación)
                // ==========================================================================
                new Reporte { NombreVendedor = "Juan Diaz", Fecha = hoy.AddDays(-2), Hora = TimeSpan.Parse("12:00"), Monto = 1000.00m, Medicamento = "Ibuprofeno", StockDisponible = 8 },
                new Reporte { NombreVendedor = "Ana Gomez", Fecha = hoy.AddDays(-4), Hora = TimeSpan.Parse("15:30"), Monto = 2500.00m, Medicamento = "Ibuprofeno", StockDisponible = 8 },

                new Reporte { NombreVendedor = "Carlos Perez", Fecha = inicioMesAnterior.AddDays(4), Hora = TimeSpan.Parse("08:15"), Monto = 1000.00m, Medicamento = "Ibuprofeno", StockDisponible = 8 },
                new Reporte { NombreVendedor = "Juan Diaz", Fecha = inicioMesAnterior.AddDays(10), Hora = TimeSpan.Parse("11:20"), Monto = 1000.00m, Medicamento = "Ibuprofeno", StockDisponible = 8 },
                new Reporte { NombreVendedor = "Sofia Castro", Fecha = inicioMesAnterior.AddDays(18), Hora = TimeSpan.Parse("15:45"), Monto = 1000.00m, Medicamento = "Ibuprofeno", StockDisponible = 8 },
                new Reporte { NombreVendedor = "Ana Gomez", Fecha = inicioMesAnterior.AddDays(25), Hora = TimeSpan.Parse("17:00"), Monto = 1000.00m, Medicamento = "Ibuprofeno", StockDisponible = 8 },             

                // ==========================================================================
                // 3. AMOXICILINA: Stock 26 (Nivel Bajo), Ventas Mes Actual: 3, Ventas Mes Anterior: 3 (0% variación)
                // ==========================================================================
                new Reporte { NombreVendedor = "Carlos Perez", Fecha = hoy.AddDays(-6), Hora = TimeSpan.Parse("10:00"), Monto = 540.00m, Medicamento = "Amoxicilina", StockDisponible = 26 },
                new Reporte { NombreVendedor = "Ana Gomez", Fecha = hoy.AddDays(-10), Hora = TimeSpan.Parse("14:30"), Monto = 540.00m, Medicamento = "Amoxicilina", StockDisponible = 26 },
                new Reporte { NombreVendedor = "Sofia Castro", Fecha = inicioMesActual.AddDays(2), Hora = TimeSpan.Parse("11:00"), Monto = 540.00m, Medicamento = "Amoxicilina", StockDisponible = 26 },

                new Reporte { NombreVendedor = "Carlos Perez", Fecha = inicioMesAnterior.AddDays(4), Hora = TimeSpan.Parse("09:15"), Monto = 540.00m, Medicamento = "Amoxicilina", StockDisponible = 26 },
                new Reporte { NombreVendedor = "Juan Diaz", Fecha = inicioMesAnterior.AddDays(15), Hora = TimeSpan.Parse("15:00"), Monto = 540.00m, Medicamento = "Amoxicilina", StockDisponible = 26 },
                new Reporte { NombreVendedor = "Sofia Castro", Fecha = inicioMesAnterior.AddDays(22), Hora = TimeSpan.Parse("16:30"), Monto = 540.00m, Medicamento = "Amoxicilina", StockDisponible = 26 },

                // ==========================================================================
                // 4. LORATADINA: Stock 60 (Suficiente), Ventas Mes Actual: 4, Ventas Mes Anterior: 0 (100% variación)
                // ==========================================================================
                new Reporte { NombreVendedor = "Ana Gomez", Fecha = hoy.AddDays(-7), Hora = TimeSpan.Parse("12:15"), Monto = 95.00m, Medicamento = "Loratadina", StockDisponible = 60 },
                new Reporte { NombreVendedor = "Juan Diaz", Fecha = hoy.AddDays(-11), Hora = TimeSpan.Parse("11:30"), Monto = 95.00m, Medicamento = "Loratadina", StockDisponible = 60 },
                new Reporte { NombreVendedor = "Carlos Perez", Fecha = inicioMesActual.AddDays(3), Hora = TimeSpan.Parse("15:45"), Monto = 95.00m, Medicamento = "Loratadina", StockDisponible = 60 },
                new Reporte { NombreVendedor = "Sofia Castro", Fecha = inicioMesActual.AddDays(6), Hora = TimeSpan.Parse("09:00"), Monto = 95.00m, Medicamento = "Loratadina", StockDisponible = 60 }
            };
        }
    }


    /// <summary>
    /// 3. EL PROXY (Proxy)
    /// </summary>
    public class ReporteDALProxy : IReporteDAL
    {
        private ReporteDALReal _reporteDALReal;
        private List<Reporte> _cacheVentas;

        public List<Reporte> ObtenerTodasLasVentas()
        {
            Console.WriteLine($"[Proxy] Solicitud de datos interceptada a las {DateTime.Now}");

            if (_cacheVentas == null)
            {
                Console.WriteLine("[Proxy] El caché está vacío. Delegando la llamada a la base de datos real...");

                if (_reporteDALReal == null)
                {
                    _reporteDALReal = new ReporteDALReal();
                }

                _cacheVentas = _reporteDALReal.ObtenerTodasLasVentas();
            }
            else
            {
                Console.WriteLine("[Proxy] ¡Ahorro de recursos! Devolviendo los datos directamente desde el caché en memoria.");
            }

            return _cacheVentas;
        }

        public void RefrescarCache()
        {
            Console.WriteLine("[Proxy] Limpiando la caché de ventas.");
            _cacheVentas = null;
        }
    }
}