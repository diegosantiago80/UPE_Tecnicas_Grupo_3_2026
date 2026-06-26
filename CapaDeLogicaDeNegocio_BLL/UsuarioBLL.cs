using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using CapaDeEntidades;
using CapaDeAccesoADatos_DAL;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL _usuarioDAL = new UsuarioDAL();
        private readonly AuditoriaBLL _auditoriaBLL = BLLFactory.CrearAuditoriaBLL();

        public List<Usuario> ObtenerUsuarios()
        {
            return _usuarioDAL.ObtenerUsuarios();
        }

        public Usuario? ObtenerPorId(int idUsuario)
        {
            return _usuarioDAL.ObtenerPorId(idUsuario);
        }

        private void ValidarReglasDeNegocio(Usuario modelo)
        {
            modelo.Nombre = Regex.Replace((modelo.Nombre ?? "").Trim(), @"\s+", " ");
            if (!Regex.IsMatch(modelo.Nombre, @"^[A-Za-zÀ-ÿÑñ]+( [A-Za-zÀ-ÿÑñ]+)*$") ||
                modelo.Nombre.Length < 2 || modelo.Nombre.Length > 15)
            {
                throw new Exception("El nombre ingresado no es valido");
            }

            modelo.Apellido = Regex.Replace((modelo.Apellido ?? "").Trim(), @"\s+", " ");
            if (!Regex.IsMatch(modelo.Apellido, @"^[A-Za-zÀ-ÿÑñ]+( [A-Za-zÀ-ÿÑñ]+)*$") ||
                modelo.Apellido.Length < 2 || modelo.Apellido.Length > 15)
            {
                throw new Exception("El apellido ingresado no es valido");
            }

            if (string.IsNullOrWhiteSpace(modelo.Telefono) || !Regex.IsMatch(modelo.Telefono, @"^[0-9]{8,15}$"))
            {
                throw new Exception("El telefono ingresado no es valido");
            }

            if (string.IsNullOrWhiteSpace(modelo.Email) || modelo.Email.Length < 5 || modelo.Email.Length > 50 ||
                !Regex.IsMatch(modelo.Email, @"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z0-9.-]+$"))
            {
                throw new Exception("El email ingresado no es valido");
            }

            if (string.IsNullOrWhiteSpace(modelo.Dni) || !Regex.IsMatch(modelo.Dni ?? "", @"^[0-9]{7,8}$"))
            {
                throw new Exception("El DNI ingresado no es valido");
            }

            if (string.IsNullOrWhiteSpace(modelo.NombreUsuario) || !Regex.IsMatch(modelo.NombreUsuario ?? "", @"^[A-Za-z0-9]{5,10}$"))
            {
                throw new Exception("El nombre de usuario ingresado no es valido");
            }

            if (!string.IsNullOrEmpty(modelo.Contrasena) && !Regex.IsMatch(modelo.Contrasena, @"^[A-Za-z0-9]{3,8}$"))
            {
                throw new Exception("La contraseña ingresada no es valida");
            }

            var (valido, mensaje) = _usuarioDAL.ValidarUnicidad(modelo.NombreUsuario?? "", modelo.Dni ?? "", modelo.IdUsuario);
            if (!valido)
            {
                throw new Exception(mensaje);
            }
        }

        public void AgregarUsuario(Usuario nuevo, int idUsuarioLogueado)
        {
            ValidarReglasDeNegocio(nuevo);
            _usuarioDAL.AgregarUsuario(nuevo);

            // Registro en Auditoría
            _auditoriaBLL.Registrar(idUsuarioLogueado, "Gestión de Usuarios", $"Registro del usuario: {nuevo.NombreUsuario}");
        }

        public void ActualizarUsuario(Usuario editado, int idUsuarioLogueado)
        {
            // si recibe los 8 asteriscos falsos, la bd no cambia la contrasena
            if (editado.Contrasena == "********")
                editado.Contrasena = "";

            ValidarReglasDeNegocio(editado);

            _usuarioDAL.ActualizarUsuario(editado);

            // Registro en Auditoría
            _auditoriaBLL.Registrar(idUsuarioLogueado, "Gestión de Usuarios", $"Modificación del usuario: {editado.NombreUsuario}");
        }

        public void EliminarUsuario(int idUsuario, int idUsuarioLogueado)
        {
            var usuario = _usuarioDAL.ObtenerPorId(idUsuario);
            if (usuario == null) return;

            // si el usuario a eliminar es administrador, verificar que quede al menos uno activo
            const int ID_PERFIL_ADMIN = 1;
            if (usuario.IdPerfil == ID_PERFIL_ADMIN)
            {
                int adminsActivos = _usuarioDAL.ObtenerUsuarios()
                    .Count(u => u.IdPerfil == ID_PERFIL_ADMIN && u.Activo && u.IdUsuario != idUsuario);

                if (adminsActivos == 0)
                    throw new Exception("No se puede eliminar al único administrador activo del sistema.");
            }

            _usuarioDAL.EliminarUsuario(idUsuario);

            // actualiza el estado EsEmpleado a false cuando el usuario es dado de baja como empleado
            var clienteDAL = new ClienteDAL();
            var clienteExistente = clienteDAL.BuscarPorDni(usuario.Dni);
            if (clienteExistente != null && clienteExistente.EsEmpleado)
            {
                clienteExistente.EsEmpleado = false;
                clienteDAL.Actualizar(clienteExistente);
            }

            // Registro en Auditoría
            _auditoriaBLL.Registrar(idUsuarioLogueado, "Gestión de Usuarios", $"Eliminación del usuario: {usuario.NombreUsuario}");
        }
    }
}
