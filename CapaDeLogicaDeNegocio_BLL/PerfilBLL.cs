using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class PerfilBLL
    {
        private readonly PerfilDAL _perfilDAL = new PerfilDAL();
        private readonly AuditoriaBLL _auditoriaBLL = BLLFactory.CrearAuditoriaBLL();

        public List<Perfil> ObtenerTodos() => _perfilDAL.ObtenerTodos();

        public Perfil? ObtenerPorId(int idPerfil) => _perfilDAL.ObtenerPorId(idPerfil);

        public (bool exito, string mensaje) Guardar(Perfil perfil, int idUsuarioLogueado)
        {
            if (string.IsNullOrWhiteSpace(perfil.Descripcion))
            {
                return (false, "La descripción del perfil es obligatoria");
            }
                
            perfil.Descripcion = Regex.Replace(perfil.Descripcion.Trim(), @"\s+", " ");

            if (perfil.Descripcion.Length < 1 || perfil.Descripcion.Length > 15 ||
                !Regex.IsMatch(perfil.Descripcion, @"^[A-Za-z0-9]+( [A-Za-z0-9]+)*$"))
            {
                return (false, "La descripción ingresada no es valida");
            }

            if (string.IsNullOrWhiteSpace(perfil.PermisosAsignados))
            {
                return (false, "¡Atención! El perfil debe tener al menos un permiso habilitado para poder funcionar.");
            }
                
            // REGLA DE SEGURIDAD: proteger los permisos al unico perfil administrador existente
            if (perfil.IdPerfil == 1)
            {
                if (!perfil.PermisosAsignados.Contains("GestionarUsuarios") || !perfil.PermisosAsignados.Contains("ConfigurarPerfiles"))
                {
                    return (false, "Por seguridad del sistema, el unico perfil administrador no puede perder sus permisos.");
                }
            }

            var (valido, mensajeUnicidad) = _perfilDAL.ValidarUnicidad(perfil.Descripcion, perfil.IdPerfil);
            if (!valido)
            {
                return (false, "¡ALERTA! El perfil descrito ya existe.");
            }
                
            bool esNuevo = perfil.IdPerfil == 0;
            var resultado = esNuevo ? _perfilDAL.Agregar(perfil) : _perfilDAL.Modificar(perfil);

            if (resultado.exito)
            {
                AuthService.Instance.RecargarPerfilesDesdeBaseDeDatos();

                // Registro auditoria
                string accion = esNuevo
                    ? $"Registró el perfil: {perfil.Descripcion}"
                    : $"Modificó el perfil: {perfil.Descripcion}";
                _auditoriaBLL.Registrar(idUsuarioLogueado, "Gestión de Perfiles", accion);
            }

            return resultado;
        }

        public (bool exito, string mensaje) Eliminar(int idPerfil, int idUsuarioLogueado)
        {
            // REGLA DE SEGURIDAD: denegar la eliminacion al unico perfil administrador existente
            if (idPerfil == 1)
            {
                return (false, "Operación denegada. El unico perfil administrador del sistema no puede ser eliminado.");
            }
                
            if (_perfilDAL.PerfilEnUso(idPerfil))
            {
                return (false, "¡Error! El perfil seleccionado está en uso por uno o más usuarios.");
            }
                
            // obtenemos el nombre antes de eliminar para dejarlo en auditoría
            var perfil = _perfilDAL.ObtenerPorId(idPerfil);

            var resultado = _perfilDAL.Eliminar(idPerfil);

            if (resultado.exito)
            {
                AuthService.Instance.RecargarPerfilesDesdeBaseDeDatos();

                // Registro auditoria
                string nombre = perfil?.Descripcion ?? idPerfil.ToString();
                _auditoriaBLL.Registrar(idUsuarioLogueado, "Gestión de Perfiles", $"Eliminó el perfil: {nombre}");
            }

            return resultado;
        }
    }
}