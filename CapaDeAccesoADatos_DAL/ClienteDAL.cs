using System;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaDeEntidades;

namespace CapaDeAccesoADatos_DAL
{
    public class ClienteDAL
    {
        public Cliente? BuscarPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return null;

            using (SqlConnection conexion = new SqlConnection(Conexion.CadenaDeConexion))
            {
                SqlCommand cmd = new SqlCommand("SP_BuscarClientePorDni", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Dni", dni);

                try
                {
                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())

                    {
                        if (dr.Read())
                        {
                            return new Cliente
                            {
                                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                Nombre = dr["Nombre"].ToString() ?? string.Empty,
                                Apellido = dr["Apellido"].ToString() ?? string.Empty,
                                Dni = dr["Dni"].ToString() ?? string.Empty,
                                Telefono = dr["Telefono"].ToString() ?? string.Empty,
                                Email = dr["Email"].ToString() ?? string.Empty,
                                ObraSocial = dr["ObraSocial"].ToString() ?? string.Empty,
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                EsEmpleado = Convert.ToBoolean(dr["EsEmpleado"]) 
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en la DAL al buscar cliente: " + ex.Message);
                }
            }
            return null;
        }


        // verifica si un DNI ya existe en la tabla Persona (empleados del sistema)
        public bool PersonaExistePorDni(string dni)
        {
            using (SqlConnection conexion = new SqlConnection(Conexion.CadenaDeConexion))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Persona WHERE DNI = @Dni", conexion);
                cmd.Parameters.AddWithValue("@Dni", dni);
                conexion.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // crea cliente para una persona que NO existe en Persona (cliente externo)
        public bool Crear(Cliente nuevoCliente)
        {
            using (SqlConnection conexion = new SqlConnection(Conexion.CadenaDeConexion))
            {
                SqlCommand cmd = new SqlCommand("SP_CrearCliente", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre",     nuevoCliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido",   nuevoCliente.Apellido);
                cmd.Parameters.AddWithValue("@Dni",        nuevoCliente.Dni);
                cmd.Parameters.AddWithValue("@Telefono",   nuevoCliente.Telefono);
                cmd.Parameters.AddWithValue("@Email",      nuevoCliente.Email);
                cmd.Parameters.AddWithValue("@ObraSocial", nuevoCliente.ObraSocial);
                cmd.Parameters.AddWithValue("@Activo",     nuevoCliente.Activo);

                conexion.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        nuevoCliente.IdCliente = Convert.ToInt32(dr[0]);
                        return true;
                    }
                }
                return false;
            }
        }

        // vincula como cliente a una persona que YA existe en Persona (empleado del sistema)
        // el SP setea EsEmpleado = 1 automaticamente
        public bool VincularEmpleadoComoCliente(Cliente nuevoCliente)
        {
            using (SqlConnection conexion = new SqlConnection(Conexion.CadenaDeConexion))
            {
                SqlCommand cmd = new SqlCommand("SP_VincularEmpleadoComoCliente", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Dni",        nuevoCliente.Dni);
                cmd.Parameters.AddWithValue("@ObraSocial", nuevoCliente.ObraSocial);
                cmd.Parameters.AddWithValue("@Activo",     nuevoCliente.Activo);

                conexion.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        nuevoCliente.IdCliente  = Convert.ToInt32(dr[0]);
                        nuevoCliente.EsEmpleado = true;
                        return true;
                    }
                }
                return false;
            }
        }

        
        public bool Actualizar(Cliente clienteModificado)
        {
            using (SqlConnection conexion = new SqlConnection(Conexion.CadenaDeConexion))
            {
                SqlCommand cmd = new SqlCommand("SP_ModificarCliente", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", clienteModificado.IdCliente);
                cmd.Parameters.AddWithValue("@Nombre", clienteModificado.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", clienteModificado.Apellido);
                cmd.Parameters.AddWithValue("@Dni", clienteModificado.Dni);
                cmd.Parameters.AddWithValue("@Telefono", clienteModificado.Telefono);
                cmd.Parameters.AddWithValue("@Email", clienteModificado.Email);
                cmd.Parameters.AddWithValue("@ObraSocial", clienteModificado.ObraSocial);
                cmd.Parameters.AddWithValue("@Activo", clienteModificado.Activo);
                cmd.Parameters.AddWithValue("@EsEmpleado", clienteModificado.EsEmpleado); // Mapeado directo

                try
                {
                    conexion.Open();
                    int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());
                    return filasAfectadas > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}