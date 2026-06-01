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

        public int IdPersona
        {
            get
            {
                return _idPersona;
            }
            set
            {
                _idPersona = value;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
            set
            {
                _nombre = value;
            }
        }

        public string Apellido
        {
            get
            {
                return _apellido;
            }
            set
            {
                _apellido = value;
            }
        }

        public string Dni
        {
            get
            {
                return _dni;
            }
            set
            {
                _dni = value;
            }
        }

        public string Telefono
        {
            get
            {
                return _telefono;
            }
            set
            {
                _telefono = value;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }

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