using CapaDeEntidades;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace CapaDeAccesoADatos_DAL
{
    public class CategoriaDAL
    {
        public List<Categoria> ObtenerTodos()
        {
            var lista = new List<Categoria>();
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RecuperarCategorias", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Categoria(
                                Convert.ToInt32(dr["IdCategoria"]),
                                dr["Nombre"].ToString() ?? string.Empty,
                                dr["Descripcion"].ToString() ?? string.Empty
                            ));
                        }
                    }
                }
                catch { }
            }
            return lista;
        }
    }
}
