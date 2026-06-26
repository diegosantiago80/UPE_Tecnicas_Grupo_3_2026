using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CapaDeAccesoADatos_DAL
{
    public class AutenticacionDAL
    {
        // valida credenciales contra la BD y devuelve los datos del usuario si son correctas
        public (bool encontrado, bool activo, int idUsuario, int idPerfil, string nombre, string apellido, string nombreUsuario, string dni) ValidarCredenciales(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_ValidarUsuario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario",    username.Trim());
                cmd.Parameters.AddWithValue("@Contrasena", password.Trim());

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return (
                            encontrado:    true,
                            activo:        Convert.ToBoolean(dr["Activo"]),
                            idUsuario:     Convert.ToInt32(dr["IdUsuario"]),
                            idPerfil:      Convert.ToInt32(dr["IdPerfil"]),
                            nombre:        dr["Nombre"]?.ToString() ?? string.Empty,
                            apellido:      dr["Apellido"]?.ToString() ?? string.Empty,
                            nombreUsuario: dr["NombreUsuario"]?.ToString() ?? string.Empty,
                            dni:           dr["DNI"]?.ToString() ?? string.Empty
                        );
                    }
                }
            }
            return (false, false, 0, 0, string.Empty, string.Empty, string.Empty, string.Empty);
        }
    }
}
