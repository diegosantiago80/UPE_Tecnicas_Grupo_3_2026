using System;

namespace CapaDeEntidades
{
    public class Cliente : Persona
    {
        private int _idCliente;
        private string _obraSocial = string.Empty;
        private bool _activo;
        private bool _esEmpleado;

        public int IdCliente
        {
            get
            {
                return _idCliente;
            }
            set
            {
                _idCliente = value;
            }
        }

        public string ObraSocial
        {
            get
            {
                return _obraSocial;
            }
            set
            {
                _obraSocial = value;
            }
        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
            set
            {
                _activo = value;
            }
        }

        public bool EsEmpleado
        {
            get
            {
                return _esEmpleado;
            }
            set
            {
                _esEmpleado = value;
            }
        }

        public string NombreCompleto
        {
            get
            {
                return $"{Nombre} {Apellido}";
            }
        }

        public Cliente() : base() { }

        public Cliente(int idCliente, string nombre, string apellido, string dni, string telefono, string email, string obraSocial, bool activo, bool esEmpleado)
            : base(0, nombre, apellido, dni, telefono, email)
        {
            _idCliente = idCliente;
            _obraSocial = obraSocial;
            _activo = activo;
            _esEmpleado = esEmpleado;
        }
    }
}