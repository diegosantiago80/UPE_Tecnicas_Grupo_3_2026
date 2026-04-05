using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Usuario
    {
        private int _idUsuario;
        private string _nombreUsuario = string.Empty;
        private string _contrasena = string.Empty;
        private string _nombre = string.Empty;
        private string _apellido = string.Empty;
        private int _idPerfil;
        private bool _activo;

        public int IdUsuario { get => _idUsuario; set => _idUsuario = value; }
        public string NombreUsuario { get => _nombreUsuario; set => _nombreUsuario = value; }
        public string Contrasena { get => _contrasena; set => _contrasena = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }
        public string Apellido { get => _apellido; set => _apellido = value; }
        public int IdPerfil { get => _idPerfil; set => _idPerfil = value; }
        public bool Activo { get => _activo; set => _activo = value; }

        public string NombreCompleto { get => $"{_nombre} {_apellido}"; }

        public Usuario() { }

        public Usuario(int idUsuario, string nombreUsuario, string contrasena, string nombre, string apellido, int idPerfil, bool activo)
        {
            _idUsuario = idUsuario;
            _nombreUsuario = nombreUsuario;
            _contrasena = contrasena;
            _nombre = nombre;
            _apellido = apellido;
            _idPerfil = idPerfil;
            _activo = activo;
        }
    }
}