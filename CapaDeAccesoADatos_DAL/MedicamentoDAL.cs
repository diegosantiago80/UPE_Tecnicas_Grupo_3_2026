using CapaDeEntidades;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace CapaDeAccesoADatos_DAL
{
    public class MedicamentoDAL
    {
        // mapea un registro del datareader a la entidad medicamento
        private Medicamento MapearMedicamento(SqlDataReader dr)
        {
            return new Medicamento(
                Convert.ToInt32(dr["IdMedicamento"]),
                dr["Nombre"].ToString() ?? string.Empty,
                dr["Descripcion"].ToString() ?? string.Empty,
                Convert.ToDecimal(dr["PrecioVenta"]),
                Convert.ToDecimal(dr["PrecioCompra"]),
                Convert.ToInt32(dr["StockActual"]),
                Convert.ToInt32(dr["StockMinimo"]),
                Convert.ToBoolean(dr["RequiereReceta"]),
                Convert.ToInt32(dr["IdCategoria"]),
                Convert.ToInt32(dr["IdLaboratorio"]),
                Convert.ToBoolean(dr["Activo"])
            );
        }

        public List<Medicamento> ObtenerTodos()
        {
            var lista = new List<Medicamento>();
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RecuperarMedicamentos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                    while (dr.Read()) lista.Add(MapearMedicamento(dr));
            }
            return lista;
        }

        public List<Medicamento> ObtenerActivos()
        {
            var lista = new List<Medicamento>();
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RecuperarMedicamentosActivos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                    while (dr.Read()) lista.Add(MapearMedicamento(dr));
            }
            return lista;
        }

        public Medicamento? ObtenerPorId(int idMedicamento)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_ObtenerMedicamentoPorId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMedicamento", idMedicamento);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read()) return MapearMedicamento(dr);
                }
            }
            return null;
        }

        public List<Medicamento> ObtenerCriticos()
        {
            var lista = new List<Medicamento>();
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RecuperarMedicamentosCriticos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                    while (dr.Read()) lista.Add(MapearMedicamento(dr));
            }
            return lista;
        }

        public (bool exito, string mensaje) Agregar(Medicamento m)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_AgregarMedicamento", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLaboratorio",  m.IdLaboratorio);
                cmd.Parameters.AddWithValue("@IdCategoria",    m.IdCategoria);
                cmd.Parameters.AddWithValue("@Nombre",         m.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion",    m.Descripcion);
                cmd.Parameters.AddWithValue("@PrecioVenta",    m.PrecioVenta);
                cmd.Parameters.AddWithValue("@PrecioCompra",   m.PrecioCompra);
                cmd.Parameters.AddWithValue("@StockActual",    m.StockActual);
                cmd.Parameters.AddWithValue("@StockMinimo",    m.StockMinimo);
                cmd.Parameters.AddWithValue("@RequiereReceta", m.RequiereReceta);
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            bool exito = Convert.ToInt32(dr["Resultado"]) == 1;
                            // el sp devuelve el id generado como mensaje en caso de exito
                            // usamos un texto descriptivo en su lugar
                            string mensaje = exito ? "Medicamento agregado correctamente" : (dr["Mensaje"].ToString() ?? string.Empty);
                            return (exito, mensaje);
                        }
                    }
                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            return (false, "Error inesperado");
        }

        public (bool exito, string mensaje) Modificar(Medicamento m)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_ModificarMedicamento", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMedicamento",  m.IdMedicamento);
                cmd.Parameters.AddWithValue("@IdLaboratorio",  m.IdLaboratorio);
                cmd.Parameters.AddWithValue("@IdCategoria",    m.IdCategoria);
                cmd.Parameters.AddWithValue("@Nombre",         m.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion",    m.Descripcion);
                cmd.Parameters.AddWithValue("@PrecioVenta",    m.PrecioVenta);
                // precio de compra no se envia: es historico y no se modifica
                cmd.Parameters.AddWithValue("@StockMinimo",    m.StockMinimo);
                cmd.Parameters.AddWithValue("@RequiereReceta", m.RequiereReceta);
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                            return (Convert.ToInt32(dr["Resultado"]) == 1, dr["Mensaje"].ToString() ?? string.Empty);
                    }
                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            return (false, "Error inesperado");
        }

        public (bool exito, string mensaje) DarDeBaja(int idMedicamento)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_DarDeBajaMedicamento", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMedicamento", idMedicamento);
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                            return (Convert.ToInt32(dr["Resultado"]) == 1, dr["Mensaje"].ToString() ?? string.Empty);
                    }
                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            return (false, "Error inesperado");
        }
    }
}
