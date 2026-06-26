using CapaDeEntidades;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CapaDeAccesoADatos_DAL
{
    public class PerfilDAL
    {
        public List<Perfil> ObtenerTodos()
        {
            var lista = new List<Perfil>();
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RecuperarPerfiles", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Perfil
                        {
                            IdPerfil = Convert.ToInt32(dr["IdPerfil"]),
                            Descripcion = dr["Descripcion"].ToString() ?? string.Empty,
                            PermisosAsignados = dr["PermisosAsignados"].ToString() ?? string.Empty
                        });
                    }
                }
            }
            return lista;
        }

        public Perfil? ObtenerPorId(int idPerfil)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_ObtenerPerfilPorId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Perfil
                        {
                            IdPerfil = Convert.ToInt32(dr["IdPerfil"]),
                            Descripcion = dr["Descripcion"].ToString() ?? string.Empty,
                            PermisosAsignados = dr["PermisosAsignados"].ToString() ?? string.Empty
                        };
                    }
                }
            }
            return null;
        }

        public (bool valido, string mensaje) ValidarUnicidad(string descripcion, int idPerfil = 0)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_ValidarUnicidadPerfil", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return (Convert.ToInt32(dr["Resultado"]) == 1, dr["Mensaje"].ToString() ?? string.Empty);
                    }
                }
            }
            return (false, "Error al validar la unicidad.");
        }

        public bool PerfilEnUso(int idPerfil)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_PerfilEnUso", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public (bool exito, string mensaje) Agregar(Perfil p)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_AgregarPerfil", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion);
                cmd.Parameters.AddWithValue("@PermisosAsignados", p.PermisosAsignados);
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return (Convert.ToInt32(dr["Resultado"]) == 1, dr["Mensaje"].ToString() ?? string.Empty);
                        }
                    }
                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            return (false, "Error al guardar el perfil.");
        }

        public (bool exito, string mensaje) Modificar(Perfil p)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_ModificarPerfil", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPerfil", p.IdPerfil);
                cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion);
                cmd.Parameters.AddWithValue("@PermisosAsignados", p.PermisosAsignados);
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return (Convert.ToInt32(dr["Resultado"]) == 1, dr["Mensaje"].ToString() ?? string.Empty);
                        }
                    }
                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            return (false, "Error al modificar el perfil.");
        }

        public (bool exito, string mensaje) Eliminar(int idPerfil)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_EliminarPerfil", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return (Convert.ToInt32(dr["Resultado"]) == 1, dr["Mensaje"].ToString() ?? string.Empty);
                        }
                    }
                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            return (false, "Error al eliminar el perfil.");
        }
    }
}