using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Cliente
    {
        private int _idCliente;
        private string _nombre = string.Empty;
        private string _apellido = string.Empty;
        private string _dni = string.Empty;
        private string _telefono = string.Empty;
        private string _email = string.Empty;
        private string _obraSocial = string.Empty;
        private bool _activo;

        public int IdCliente { get => _idCliente; set => _idCliente = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }
        public string Apellido { get => _apellido; set => _apellido = value; }
        public string Dni { get => _dni; set => _dni = value; }
        public string Telefono { get => _telefono; set => _telefono = value; }
        public string Email { get => _email; set => _email = value; }
        public string ObraSocial { get => _obraSocial; set => _obraSocial = value; }
        public bool Activo { get => _activo; set => _activo = value; }

        public string NombreCompleto { get => $"{_nombre} {_apellido}"; }

        public Cliente() { }

        public Cliente(int idCliente, string nombre, string apellido, string dni, string telefono, string email, string obraSocial, bool activo)
        {
            _idCliente = idCliente;
            _nombre = nombre;
            _apellido = apellido;
            _dni = dni;
            _telefono = telefono;
            _email = email;
            _obraSocial = obraSocial;
            _activo = activo;
        }
    }
}