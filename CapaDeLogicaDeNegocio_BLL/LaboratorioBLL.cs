using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class LaboratorioBLL
    {
        private readonly LaboratorioDAL _laboratorioDAL = new LaboratorioDAL();
        private readonly AuditoriaBLL _auditoriaBLL = BLLFactory.CrearAuditoriaBLL();

        public List<Laboratorio> ObtenerTodos()
        {
            return _laboratorioDAL.ObtenerTodos();
        }

        public Laboratorio? ObtenerPorId(int idLaboratorio)
        {
            return _laboratorioDAL.ObtenerPorId(idLaboratorio);
        }

        public List<Laboratorio> ObtenerActivos()
        {
            return _laboratorioDAL.ObtenerActivos();
        }

        public (bool exito, string mensaje) Agregar(Laboratorio l, int idUsuarioLogueado)
        {
            var validacion = ValidarDatos(l);
            if (!validacion.exito) return validacion;
            var resultado = _laboratorioDAL.Agregar(l);

            // Registro auditoria
            if (resultado.exito)
                _auditoriaBLL.Registrar(idUsuarioLogueado, "Nuevo laboratorio", $"Registró el laboratorio: {l.RazonSocial}");

            return resultado;
        }

        public (bool exito, string mensaje) Modificar(Laboratorio l, int idUsuarioLogueado)
        {
            var validacion = ValidarDatos(l);
            if (!validacion.exito) return validacion;
            var resultado = _laboratorioDAL.Modificar(l);

            // Registro auditoria
            if (resultado.exito)
                _auditoriaBLL.Registrar(idUsuarioLogueado, "Editar laboratorio", $"Modificó el laboratorio: {l.RazonSocial}");

            return resultado;
        }

        /// <summary>
        /// Valida RazonSocial: solo letras, 5-20 caracteres, máximo un espacio entre palabras
        /// </summary>
        private (bool valido, string mensaje) ValidarRazonSocial(string razonSocial)
        {
            if (string.IsNullOrWhiteSpace(razonSocial))
                return (false, "La razón social es obligatoria");

            razonSocial = razonSocial.Trim();

            if (razonSocial.Length < 5 || razonSocial.Length > 20)
                return (false, $"La razón social debe tener entre 5 y 20 caracteres (actual: {razonSocial.Length})");

            if (!Regex.IsMatch(razonSocial, @"^[A-Za-záéíóúÁÉÍÓÚñÑ\s]+$"))
                return (false, "La razón social solo puede contener letras y espacios");

            if (Regex.IsMatch(razonSocial, @"\s{2,}"))
                return (false, "La razón social no puede tener más de un espacio consecutivo");

            return (true, "");
        }

        /// <summary>
        /// Valida Teléfono: solo números, exactamente 10 dígitos
        /// </summary>
        private (bool valido, string mensaje) ValidarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return (false, "El teléfono es obligatorio");

            telefono = telefono.Trim();

            if (!Regex.IsMatch(telefono, @"^\d{10}$"))
                return (false, "El teléfono debe contener exactamente 10 dígitos sin espacios ni caracteres especiales");

            return (true, "");
        }

        /// <summary>
        /// Valida Email: letras, números, @, y punto (.)
        /// Formato básico: usuario@dominio.extension
        /// </summary>
        private (bool valido, string mensaje) ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "El email es obligatorio");

            email = email.Trim();

            // Validación básica de email: usuario@dominio.extension
            if (!Regex.IsMatch(email, @"^[A-Za-z0-9]+@[A-Za-z0-9]+\.[A-Za-z0-9]+$"))
                return (false, "El formato del email es inválido. Debe ser: usuario@dominio.extension");

            if (email.Length > 100)
                return (false, "El email no puede exceder 100 caracteres");

            return (true, "");
        }

        /// <summary>
        /// Valida Dirección: solo letras y números
        /// </summary>
        private (bool valido, string mensaje) ValidarDireccion(string direccion)
        {
            if (string.IsNullOrWhiteSpace(direccion))
                return (false, "La dirección es obligatoria");

            direccion = direccion.Trim();

            if (direccion.Length < 5 || direccion.Length > 100)
                return (false, $"La dirección debe tener entre 5 y 100 caracteres (actual: {direccion.Length})");

            if (!Regex.IsMatch(direccion, @"^[A-Za-z0-9\s]+$"))
                return (false, "La dirección solo puede contener letras y números");

            return (true, "");
        }

        private (bool exito, string mensaje) ValidarDatos(Laboratorio l)
        {
            // Validar Razón Social
            var validacionRazonSocial = ValidarRazonSocial(l.RazonSocial);
            if (!validacionRazonSocial.valido)
                return (false, validacionRazonSocial.mensaje);

            // Validar CUIT
            if (string.IsNullOrWhiteSpace(l.Cuit))
                return (false, "El CUIT es obligatorio");
            if (!ValidarFormatoCuit(l.Cuit))
                return (false, "El formato del CUIT es inválido. Debe ser XX-XXXXXXXX-X");

            // Validar Teléfono
            var validacionTelefono = ValidarTelefono(l.Telefono);
            if (!validacionTelefono.valido)
                return (false, validacionTelefono.mensaje);

            // Validar Email
            var validacionEmail = ValidarEmail(l.Email);
            if (!validacionEmail.valido)
                return (false, validacionEmail.mensaje);

            // Validar Dirección
            var validacionDireccion = ValidarDireccion(l.Direccion);
            if (!validacionDireccion.valido)
                return (false, validacionDireccion.mensaje);

            return (true, string.Empty);
        }

        public (bool exito, string mensaje) DarDeBaja(int idLaboratorio, int idUsuarioLogueado)
        {
            // obtenemos el nombre antes de dar de baja para dejarlo en auditoria
            var lab = _laboratorioDAL.ObtenerPorId(idLaboratorio);

            var resultado = _laboratorioDAL.DarDeBaja(idLaboratorio);

            // Registro auditoria
            if (resultado.exito)
            {
                string nombre = lab?.RazonSocial ?? idLaboratorio.ToString();
                _auditoriaBLL.Registrar(idUsuarioLogueado, "Eliminar laboratorio", $"Eliminó el laboratorio: {nombre}");
            }

            return resultado;
        }

        // valida el formato XX-XXXXXXXX-X usando regex
        private bool ValidarFormatoCuit(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit)) return false;
            return Regex.IsMatch(
                cuit.Trim(),
                @"^\d{2}-\d{8}-\d{1}$"
            );
        }
    }
}