using CapaDeEntidades;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace CapaDeAccesoADatos_DAL
{
    public class LaboratorioDAL
    {
        // mapea un registro del datareader a la entidad laboratorio
        private Laboratorio MapearLaboratorio(SqlDataReader dr)
        {
            return new Laboratorio(
                Convert.ToInt32(dr["IdLaboratorio"]),
                dr["RazonSocial"].ToString() ?? string.Empty,
                dr["Cuit"].ToString() ?? string.Empty,
                dr["Telefono"].ToString() ?? string.Empty,
                dr["Email"].ToString() ?? string.Empty,
                dr["Direccion"].ToString() ?? string.Empty,
                Convert.ToBoolean(dr["Activo"])
            );
        }

        public List<Laboratorio> ObtenerTodos()
        {
            var lista = new List<Laboratorio>();
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RecuperarLaboratorios", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                    while (dr.Read()) lista.Add(MapearLaboratorio(dr));
            }
            return lista;
        }

        public Laboratorio? ObtenerPorId(int idLaboratorio)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_ObtenerLaboratorioPorId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLaboratorio", idLaboratorio);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read()) return MapearLaboratorio(dr);
                }
            }
            return null;
        }

        public List<Laboratorio> ObtenerActivos()
        {
            var lista = new List<Laboratorio>();
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RecuperarLaboratoriosActivos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                    while (dr.Read()) lista.Add(MapearLaboratorio(dr));
            }
            return lista;
        }

        public (bool exito, string mensaje) Agregar(Laboratorio l)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_AgregarLaboratorio", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RazonSocial", l.RazonSocial);
                cmd.Parameters.AddWithValue("@Cuit",        l.Cuit);
                cmd.Parameters.AddWithValue("@Telefono",    l.Telefono);
                cmd.Parameters.AddWithValue("@Email",       l.Email);
                cmd.Parameters.AddWithValue("@Direccion",   l.Direccion);
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            bool exito = Convert.ToInt32(dr["Resultado"]) == 1;
                            string mensaje = exito ? "Laboratorio agregado correctamente" : (dr["Mensaje"].ToString() ?? string.Empty);
                            return (exito, mensaje);
                        }
                    }
                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            return (false, "Error inesperado");
        }

        public (bool exito, string mensaje) Modificar(Laboratorio l)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_ModificarLaboratorio", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLaboratorio", l.IdLaboratorio);
                cmd.Parameters.AddWithValue("@RazonSocial",   l.RazonSocial);
                cmd.Parameters.AddWithValue("@Cuit",          l.Cuit);
                cmd.Parameters.AddWithValue("@Telefono",      l.Telefono);
                cmd.Parameters.AddWithValue("@Email",         l.Email);
                cmd.Parameters.AddWithValue("@Direccion",     l.Direccion);
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return (true, "Laboratorio modificado correctamente");
                }
                catch (Exception ex) { return (false, ex.Message); }
            }
        }

        public (bool exito, string mensaje) DarDeBaja(int idLaboratorio)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_DarDeBajaLaboratorio", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLaboratorio", idLaboratorio);
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
