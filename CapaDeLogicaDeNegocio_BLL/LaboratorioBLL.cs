using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;
using System.Collections.Generic;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class LaboratorioBLL
    {
        private readonly LaboratorioDAL _laboratorioDAL = new LaboratorioDAL();

        public List<Laboratorio> ObtenerTodos()
        {
            return _laboratorioDAL.ObtenerTodos();
        }

        public List<Laboratorio> ObtenerActivos()
        {
            return _laboratorioDAL.ObtenerActivos();
        }

        public (bool exito, string mensaje) Agregar(Laboratorio l)
        {
            if (string.IsNullOrWhiteSpace(l.RazonSocial))
                return (false, "La razon social es obligatoria");
            if (string.IsNullOrWhiteSpace(l.Cuit))
                return (false, "El CUIT es obligatorio");
            if (!ValidarFormatoCuit(l.Cuit))
                return (false, "El formato del CUIT es invalido. Debe ser XX-XXXXXXXX-X");

            return _laboratorioDAL.Agregar(l);
        }

        public (bool exito, string mensaje) Modificar(Laboratorio l)
        {
            if (string.IsNullOrWhiteSpace(l.RazonSocial))
                return (false, "La razon social es obligatoria");
            if (string.IsNullOrWhiteSpace(l.Cuit))
                return (false, "El CUIT es obligatorio");
            if (!ValidarFormatoCuit(l.Cuit))
                return (false, "El formato del CUIT es invalido. Debe ser XX-XXXXXXXX-X");

            return _laboratorioDAL.Modificar(l);
        }

        public (bool exito, string mensaje) DarDeBaja(int idLaboratorio)
        {
            return _laboratorioDAL.DarDeBaja(idLaboratorio);
        }

        // valida el formato XX-XXXXXXXX-X usando regex
        private bool ValidarFormatoCuit(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(
                cuit.Trim(),
                @"^\d{2}-\d{8}-\d{1}$"
            );
        }
    }
}
