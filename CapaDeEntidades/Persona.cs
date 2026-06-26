using System;

namespace CapaDeEntidades
{
    public class Persona
    {
        private int _idPersona;
        private string _nombre = string.Empty;
        private string _apellido = string.Empty;
        private string _dni = string.Empty;
        private string _telefono = string.Empty;
        private string _email = string.Empty;

        public int IdPersona { get => _idPersona; set => _idPersona = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }
        public string Apellido { get => _apellido; set => _apellido = value; }
        public string Dni { get => _dni; set => _dni = value; }
        public string Telefono { get => _telefono; set => _telefono = value; }
        public string Email { get => _email; set => _email = value; }

        // propiedad calculada disponible para todas las subclases
        public string NombreCompleto { get => $"{_nombre} {_apellido}".Trim(); }

        public Persona() { }

        public Persona(int idPersona, string nombre, string apellido, string dni, string telefono, string email)
        {
            _idPersona = idPersona;
            _nombre = nombre;
            _apellido = apellido;
            _dni = dni;
            _telefono = telefono;
            _email = email;
        }
    }
}
