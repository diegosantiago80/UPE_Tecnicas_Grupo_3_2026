using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;
using System;
using System.Collections.Generic;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class AuditoriaBLL
    {
        private readonly AuditoriaDAL _dal = new AuditoriaDAL();
        public void Registrar(int idUsuario, string modulo, string accion) => _dal.Registrar(idUsuario, modulo, accion);
        public List<Auditoria> ObtenerLogs(DateTime? fDesde, DateTime? fHasta, string? usuario, string? modulo) => _dal.ObtenerLogs(fDesde, fHasta, usuario, modulo);
    }
}