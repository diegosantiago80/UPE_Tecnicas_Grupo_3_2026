using CapaDeEntidades;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CapaDeAccesoADatos_DAL
{
    public class AuditoriaDAL
    {
        public void Registrar(int idUsuario, string modulo, string accion)
        {
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RegistrarLogAuditoria", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@Modulo", modulo);
                cmd.Parameters.AddWithValue("@Accion", accion);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Auditoria> ObtenerLogs(DateTime? fDesde, DateTime? fHasta, string? usuario, string? modulo)
        {
            var lista = new List<Auditoria>();
            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            using (SqlCommand cmd = new SqlCommand("SP_RecuperarLogsAuditoria", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaDesde", (object?)fDesde ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaHasta", (object?)fHasta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Usuario", (object?)usuario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Modulo", (object?)modulo ?? DBNull.Value);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Auditoria
                        {
                            FechaHora = Convert.ToDateTime(dr["FechaHora"]),
                            Usuario = dr["Usuario"].ToString() ?? "",
                            Perfil = dr["Perfil"].ToString() ?? "",
                            Modulo = dr["Modulo"].ToString() ?? "",
                            Accion = dr["Accion"].ToString() ?? ""
                        });
                    }
                }
            }
            return lista;
        }
    }
}