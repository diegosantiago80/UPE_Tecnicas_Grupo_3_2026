using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaDeEntidades;

namespace CapaDeAccesoADatos_DAL
{
    public class UsuarioDAL
    {
        // obtiene la lista completa de usuarios
        public List<Usuario> ObtenerUsuarios()
        {
            var lista = new List<Usuario>();

            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("SP_ObtenerUsuarios", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                NombreUsuario = dr["NombreUsuario"].ToString() ?? string.Empty,
                                Contrasena = dr["Contrasena"].ToString() ?? string.Empty,
                                IdPerfil = Convert.ToInt32(dr["IdPerfil"]),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                Nombre = dr["Nombre"].ToString() ?? string.Empty,
                                Apellido = dr["Apellido"].ToString() ?? string.Empty,
                                Dni = dr["DNI"].ToString() ?? string.Empty,
                                Telefono = dr["Telefono"].ToString() ?? string.Empty,
                                Email = dr["Email"].ToString() ?? string.Empty
                            };
                            lista.Add(usuario);
                        }
                    }
                }
            }
            return lista;
        }

        // inserta un nuevo registro
        public void AgregarUsuario(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AgregarUsuario", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@DNI", usuario.Dni);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                    cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                    cmd.Parameters.AddWithValue("@IdPerfil", usuario.IdPerfil);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // modifica los datos de un registro existente
        public void ActualizarUsuario(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("SP_ModificarUsuario", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@DNI", usuario.Dni);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                    cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                    cmd.Parameters.AddWithValue("@IdPerfil", usuario.IdPerfil);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // baja logica
        public void EliminarUsuario(int idUsuario)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("SP_EliminarUsuario", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
