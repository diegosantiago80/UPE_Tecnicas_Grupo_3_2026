using System;

namespace CapaDeEntidades
{
    // hereda nombre, apellido, dni, telefono, email y NombreCompleto de Persona
    public class Usuario : Persona
    {
        private int _idUsuario;
        private string _nombreUsuario = string.Empty;
        private string _contrasena = string.Empty;
        private int _idPerfil;
        private bool _activo;

        public int IdUsuario { get => _idUsuario; set => _idUsuario = value; }
        public string NombreUsuario { get => _nombreUsuario; set => _nombreUsuario = value; }
        public string Contrasena { get => _contrasena; set => _contrasena = value; }
        public int IdPerfil { get => _idPerfil; set => _idPerfil = value; }
        public bool Activo { get => _activo; set => _activo = value; }

        public Usuario() : base() { }

        public Usuario(int idUsuario, string nombreUsuario, string contrasena, string nombre, string apellido, string dni, string telefono, string email, int idPerfil, bool activo)
            : base(0, nombre, apellido, dni, telefono, email)
        {
            _idUsuario = idUsuario;
            _nombreUsuario = nombreUsuario;
            _contrasena = contrasena;
            _idPerfil = idPerfil;
            _activo = activo;
        }
    }
}
