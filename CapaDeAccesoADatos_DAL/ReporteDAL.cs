using CapaDeEntidades;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CapaDeAccesoADatos_DAL
{
    public interface IReporteDAL
    {
        List<Reporte> ObtenerTodasLasVentas();
        List<Reporte> ObtenerReporteVentas(string nombreUsuario, string periodo, DateTime? fecha);
        Reporte ObtenerEstadisticasMedicamento(string nombreMedicamento, DateTime fechaInicio, DateTime fechaFin);
        List<Reporte> ObtenerEstadisticasTodosMedicamentos();
    }

    public class ReporteDALReal : IReporteDAL
    {
        public List<Reporte> ObtenerTodasLasVentas()
        {
            List<Reporte> lista = new List<Reporte>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_ReporteVentas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreUsuario", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Periodo", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fecha", DBNull.Value);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Reporte
                            {
                                IdVenta = reader["IdVenta"] != DBNull.Value ? Convert.ToInt32(reader["IdVenta"]) : 0,
                                NombreVendedor = reader["NombreVendedor"] != DBNull.Value ? reader["NombreVendedor"].ToString()! : "Desconocido",
                                Fecha = reader["FechaVenta"] != DBNull.Value ? Convert.ToDateTime(reader["FechaVenta"]) : DateTime.MinValue,
                                Medicamento = reader["Medicamento"] != DBNull.Value ? reader["Medicamento"].ToString()! : "Sin Nombre",
                                // AHORA C# RECOLECTA LAS UNIDADES VENDIDAS:
                                UnidadesVendidas = reader["Cantidad"] != DBNull.Value ? Convert.ToInt32(reader["Cantidad"]) : 0,
                                Monto = reader["Monto"] != DBNull.Value ? Convert.ToDecimal(reader["Monto"]) : 0.00m
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<Reporte> ObtenerReporteVentas(string nombreUsuario, string periodo, DateTime? fecha)
        {
            List<Reporte> lista = new List<Reporte>();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_ReporteVentas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreUsuario", string.IsNullOrEmpty(nombreUsuario) ? (object)DBNull.Value : nombreUsuario.Trim());
                    cmd.Parameters.AddWithValue("@Periodo", string.IsNullOrEmpty(periodo) ? (object)DBNull.Value : periodo.Trim());
                    cmd.Parameters.AddWithValue("@Fecha", fecha.HasValue ? (object)fecha.Value : DBNull.Value);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Reporte
                            {
                                IdVenta = reader["IdVenta"] != DBNull.Value ? Convert.ToInt32(reader["IdVenta"]) : 0,
                                NombreVendedor = reader["NombreVendedor"] != DBNull.Value ? reader["NombreVendedor"].ToString()! : "Desconocido",
                                Fecha = reader["FechaVenta"] != DBNull.Value ? Convert.ToDateTime(reader["FechaVenta"]) : DateTime.MinValue,
                                Medicamento = reader["Medicamento"] != DBNull.Value ? reader["Medicamento"].ToString()! : "Sin Nombre",
                                // AHORA C# RECOLECTA LAS UNIDADES VENDIDAS:
                                UnidadesVendidas = reader["Cantidad"] != DBNull.Value ? Convert.ToInt32(reader["Cantidad"]) : 0,
                                Monto = reader["Monto"] != DBNull.Value ? Convert.ToDecimal(reader["Monto"]) : 0.00m
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Reporte ObtenerEstadisticasMedicamento(string nombreMedicamento, DateTime fechaInicio, DateTime fechaFin)
        {
            Reporte r = new Reporte();

            using (SqlConnection conn = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_EstadisticasMedicamento", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NombreMedicamento", string.IsNullOrEmpty(nombreMedicamento) ? (object)DBNull.Value : nombreMedicamento.Trim());
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        r.Medicamento = reader["NombreMedicamento"] != DBNull.Value ? reader["NombreMedicamento"].ToString()! : "Sin Nombre";
                        r.UnidadesVendidas = reader["CantidadUnidadesVendidas"] != DBNull.Value ? Convert.ToInt32(reader["CantidadUnidadesVendidas"]) : 0;
                        r.PorcentajeVariacion = reader["PorcentajeVsMesAnterior"] != DBNull.Value ? Convert.ToDecimal(reader["PorcentajeVsMesAnterior"]) : 0.00m;
                        r.StockDisponible = reader["UnidadesEnStock"] != DBNull.Value ? Convert.ToInt32(reader["UnidadesEnStock"]) : 0;
                    }
                }
            }
            return r;
        }

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
                            int unidadesActuales = reader["CantidadUnidadesVendidas"] != DBNull.Value ? Convert.ToInt32(reader["CantidadUnidadesVendidas"]) : 0;
                            int unidadesAnteriores = reader["UnidadesMesAnterior"] != DBNull.Value ? Convert.ToInt32(reader["UnidadesMesAnterior"]) : 0;
                            int stock = reader["UnidadesEnStock"] != DBNull.Value ? Convert.ToInt32(reader["UnidadesEnStock"]) : 0;

                            decimal variacion = 0;
                            if (unidadesAnteriores > 0)
                            {
                                variacion = ((decimal)(unidadesActuales - unidadesAnteriores) / unidadesAnteriores) * 100;
                            }
                            else if (unidadesActuales > 0)
                            {
                                variacion = 100;
                            }

                            lista.Add(new Reporte
                            {
                                Medicamento = reader["NombreMedicamento"] != DBNull.Value ? reader["NombreMedicamento"].ToString()! : "Sin Nombre",
                                UnidadesVendidas = unidadesActuales,
                                PorcentajeVariacion = variacion,
                                StockDisponible = stock
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }

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

        public List<Reporte> ObtenerEstadisticasTodosMedicamentos()
        {
            return _objetoReal.ObtenerEstadisticasTodosMedicamentos();
        }
    }
}