using System;
using System.Collections.Generic;
using System.Linq;
using CapaDeEntidades;
using CapaDeAccesoADatos_DAL;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL _usuarioDAL;

        public UsuarioBLL()
        {
            _usuarioDAL = new UsuarioDAL();
        }

        public List<Usuario> ObtenerUsuarios()
        {
            return _usuarioDAL.ObtenerUsuarios();
        }

        private void ValidarReglasDeNegocio(Usuario modelo)
        {
            // validacion del formato de email
            if (string.IsNullOrWhiteSpace(modelo.Email) || !modelo.Email.Contains("@") || !modelo.Email.Contains("."))
                throw new Exception("Ese mail ingresado no es valido");

            var usuariosActuales = _usuarioDAL.ObtenerUsuarios();

            // usuario unico (si esta activo)
            bool existeDuplicadoActivo = usuariosActuales.Any(u =>
                u.NombreUsuario.Equals(modelo.NombreUsuario, StringComparison.OrdinalIgnoreCase)
                && u.Activo == true
                && u.IdUsuario != modelo.IdUsuario);

            if (existeDuplicadoActivo)
                throw new Exception("El nombre de usuario ingresado ya existe. Por favor, elija otro.");

            // usuario unico (si esta dado de baja)
            bool existeInactivoUsuario = usuariosActuales.Any(u =>
                u.NombreUsuario.Equals(modelo.NombreUsuario, StringComparison.OrdinalIgnoreCase)
                && u.Activo == false
                && u.IdUsuario != modelo.IdUsuario);

            if (existeInactivoUsuario)
                throw new Exception("Ese nombre de usuario no es valido");

            // dni unico (si esta activo)
            bool existeDni = usuariosActuales.Any(u =>
                u.Dni == modelo.Dni
                && u.Activo == true
                && u.IdUsuario != modelo.IdUsuario);

            if (existeDni)
                throw new Exception("El DNI ingresado ya se encuentra registrado en el sistema.");

            // dni unico (si esta dado de baja)
            bool existeInactivoDni = usuariosActuales.Any(u =>
                u.Dni == modelo.Dni
                && u.Activo == false
                && u.IdUsuario != modelo.IdUsuario);

            if (existeInactivoDni)
                throw new Exception("Ese numero de DNI no es valido");
        }

        public void AgregarUsuario(Usuario nuevo)
        {
            ValidarReglasDeNegocio(nuevo);
            _usuarioDAL.AgregarUsuario(nuevo);
        }

        public void ActualizarUsuario(Usuario editado)
        {
            ValidarReglasDeNegocio(editado);

            // si recibe los 8 asteriscos falsos, la bd no cambia la contrasena
            if (editado.Contrasena == "********")
                editado.Contrasena = "";

            _usuarioDAL.ActualizarUsuario(editado);
        }

        public void EliminarUsuario(int idUsuario)
        {
            _usuarioDAL.EliminarUsuario(idUsuario);
        }
    }
}
