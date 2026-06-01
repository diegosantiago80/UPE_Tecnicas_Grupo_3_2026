using CapaDeEntidades;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace CapaDeAccesoADatos_DAL
{
    public class CompraDAL
    {
        // registra la compra y sus detalles dentro de una transaccion atomica
        // si falla cualquier paso se hace rollback completo
        public (bool exito, int idCompra, string mensaje) RegistrarCompra(Compra compra)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            {
                con.Open();
                SqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // paso 1: insertar la cabecera de la compra
                    int idCompraGenerado = 0;
                    using (SqlCommand cmdCompra = new SqlCommand("SP_RegistrarCompra", con, transaccion))
                    {
                        cmdCompra.CommandType = CommandType.StoredProcedure;
                        cmdCompra.Parameters.AddWithValue("@IdLaboratorio", compra.IdLaboratorio);
                        cmdCompra.Parameters.AddWithValue("@IdUsuario",     compra.IdUsuario);
                        cmdCompra.Parameters.AddWithValue("@Total",         compra.Total);

                        using (SqlDataReader dr = cmdCompra.ExecuteReader())
                        {
                            if (dr.Read())
                                idCompraGenerado = Convert.ToInt32(dr["IdCompra"]);
                        }
                    }

                    if (idCompraGenerado == 0)
                    {
                        transaccion.Rollback();
                        return (false, 0, "No se pudo generar la compra");
                    }

                    // paso 2: insertar cada detalle y actualizar stock
                    foreach (var detalle in compra.Detalle)
                    {
                        using (SqlCommand cmdDetalle = new SqlCommand("SP_AgregarDetalleCompra", con, transaccion))
                        {
                            cmdDetalle.CommandType = CommandType.StoredProcedure;
                            cmdDetalle.Parameters.AddWithValue("@IdCompra",       idCompraGenerado);
                            cmdDetalle.Parameters.AddWithValue("@IdMedicamento",  detalle.IdMedicamento);
                            cmdDetalle.Parameters.AddWithValue("@Cantidad",       detalle.Cantidad);
                            cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                            cmdDetalle.ExecuteNonQuery();
                        }
                    }

                    transaccion.Commit();
                    return (true, idCompraGenerado, "Compra registrada correctamente");
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    return (false, 0, ex.Message);
                }
            }
        }
    }
}
