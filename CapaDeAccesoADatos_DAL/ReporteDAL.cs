using CapaDeEntidades;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CapaDeAccesoADatos_DAL
{
    // ====================================================================================
    // CONTRATO: Exigido por la BLL
    // ====================================================================================
    public interface IReporteDAL
    {
        List<Reporte> ObtenerTodasLasVentas();
        List<Reporte> ObtenerReporteVentas(string nombreUsuario, string periodo, DateTime? fecha);
        Reporte ObtenerEstadisticasMedicamento(string nombreMedicamento, DateTime fechaInicio, DateTime fechaFin);
        List<Reporte> ObtenerEstadisticasTodosMedicamentos();
    }

    // ====================================================================================
    // SUJETO REAL: Va a la base de datos real con ADO.NET (Conexión Centralizada)
    // ====================================================================================
    public class ReporteDALReal : IReporteDAL
    {
        // 1. Método para la grilla de Reporte de Ventas (VerReporte)
        public List<Reporte> ObtenerReporteVentas(string nombreUsuario, string periodo, DateTime? fecha)
        {
            List<Reporte> lista = new List<Reporte>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_ReporteVentas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Periodo", string.IsNullOrEmpty(periodo) ? (object)DBNull.Value : periodo);
                    cmd.Parameters.AddWithValue("@Fecha", fecha.HasValue ? (object)fecha.Value : DBNull.Value);

                    conn.Open();
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Instanciamos el objeto vacío con tu constructor por defecto
                            Reporte r = new Reporte();

                            // Mapeamos directo a tus propiedades públicas
                            r.IdVenta = Convert.ToInt32(reader["IdVenta"]);
                            r.NombreVendedor = reader["NombreVendedor"].ToString();
                            r.Fecha = Convert.ToDateTime(reader["FechaVenta"]);
                            r.Medicamento = reader["Medicamento"].ToString();
                            r.Monto = Convert.ToDecimal(reader["Monto"]);

                            lista.Add(r);
                        }
                    }
                }
            }
            return lista;
        }

        // 2. Método para los Badges de las Estadísticas (GenerarEstadistica)
        public Reporte ObtenerEstadisticasMedicamento(string nombreMedicamento, DateTime fechaInicio, DateTime fechaFin)
        {
            Reporte r = null;

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_EstadisticasMedicamento", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreMedicamento", nombreMedicamento);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            r = new Reporte();

                            // Mapeamos usando las propiedades exactas de tu clase Reporte
                            r.Medicamento = reader["NombreMedicamento"].ToString();
                            r.UnidadesVendidas = Convert.ToInt32(reader["CantidadUnidadesVendidas"]);
                            r.PorcentajeVariacion = Convert.ToDecimal(reader["PorcentajeVsMesAnterior"]);
                            r.StockDisponible = Convert.ToInt32(reader["UnidadesEnStock"]);
                        }
                    }
                }
            }
            return r;
        }

        // metodo complementario exigido por la interfaz
        public List<Reporte> ObtenerTodasLasVentas()
        {
            return new List<Reporte>();
        }

        // llama al SP que devuelve estadisticas de todos los medicamentos activos de una sola vez
        public List<Reporte> ObtenerEstadisticasTodosMedicamentos()
        {
            var lista = new List<Reporte>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_EstadisticasTodosMedicamentos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int unidadesActuales   = Convert.ToInt32(reader["CantidadUnidadesVendidas"]);
                            int unidadesAnteriores = Convert.ToInt32(reader["UnidadesMesAnterior"]);
                            int stock              = Convert.ToInt32(reader["UnidadesEnStock"]);

                            // calcular porcentaje de variacion en C# igual que el SP individual
                            decimal variacion = 0;
                            if (unidadesAnteriores > 0)
                                variacion = ((decimal)(unidadesActuales - unidadesAnteriores) / unidadesAnteriores) * 100;
                            else if (unidadesActuales > 0)
                                variacion = 100;

                            lista.Add(new Reporte
                            {
                                Medicamento       = reader["NombreMedicamento"].ToString() ?? string.Empty,
                                UnidadesVendidas  = unidadesActuales,
                                PorcentajeVariacion = variacion,
                                StockDisponible   = stock
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }

    // ====================================================================================
    // PROXY
    // ====================================================================================
    public class ReporteDALProxy : IReporteDAL
    {
        private static List<Reporte> _cacheVentas = null;
        private readonly IReporteDAL _objetoReal = new ReporteDALReal();

        public List<Reporte> ObtenerReporteVentas(string nombreUsuario, string periodo, DateTime? fecha)
        {
            return _objetoReal.ObtenerReporteVentas(nombreUsuario, periodo, fecha);
        }

        public Reporte ObtenerEstadisticasMedicamento(string nombreMedicamento, DateTime fechaInicio, DateTime fechaFin)
        {
            return _objetoReal.ObtenerEstadisticasMedicamento(nombreMedicamento, fechaInicio, fechaFin);
        }

        public List<Reporte> ObtenerTodasLasVentas()
        {
            if (_cacheVentas == null)
                _cacheVentas = _objetoReal.ObtenerTodasLasVentas();
            return _cacheVentas;
        }

        // estadisticas siempre van directo a la BD, sin cache (el stock cambia con cada venta)
        public List<Reporte> ObtenerEstadisticasTodosMedicamentos()
        {
            return _objetoReal.ObtenerEstadisticasTodosMedicamentos();
        }
    }
}