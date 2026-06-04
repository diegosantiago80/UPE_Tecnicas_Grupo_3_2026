using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CapaDeEntidades;

namespace CapaDeAccesoADatos_DAL
{
    public class VentaDAL
    {
        // registra la venta y su detalle usando el SP de Victoria
        // devuelve el IdVenta generado por la BD
        public int RegistrarVenta(int idCliente, int idUsuarioVendedor, decimal total, List<DetalleVenta> detalle)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("SP_RegistrarVenta", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    cmd.Parameters.AddWithValue("@IdUsuarioVendedor", idUsuarioVendedor);
                    cmd.Parameters.AddWithValue("@Total", total);

                    // el SP espera un parametro de tipo tabla TipoDetalleVenta
                    DataTable tablaDetalle = ConstruirTablaDetalle(detalle);
                    SqlParameter paramDetalle = cmd.Parameters.AddWithValue("@Detalle", tablaDetalle);
                    paramDetalle.SqlDbType = SqlDbType.Structured;
                    paramDetalle.TypeName = "dbo.TipoDetalleVenta";

                    con.Open();
                    // el SP retorna el IdVenta generado
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
        }

        // construye el DataTable con la estructura que espera TipoDetalleVenta
        private DataTable ConstruirTablaDetalle(List<DetalleVenta> detalle)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("IdMedicamento",  typeof(int));
            dt.Columns.Add("Cantidad",       typeof(int));
            dt.Columns.Add("PrecioUnitario", typeof(decimal));

            foreach (var item in detalle)
            {
                dt.Rows.Add(item.IdMedicamento, item.Cantidad, item.PrecioUnitario);
            }
            return dt;
        }
    }
}
