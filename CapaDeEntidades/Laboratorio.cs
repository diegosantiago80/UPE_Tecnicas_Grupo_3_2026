using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Laboratorio
    {
        private int _idLaboratorio;
        private string _razonSocial = string.Empty;
        private string _cuit = string.Empty;
        private string _telefono = string.Empty;
        private string _email = string.Empty;
        private string _direccion = string.Empty;
        private bool _activo;

        public int IdLaboratorio { get => _idLaboratorio; set => _idLaboratorio = value; }
        public string RazonSocial { get => _razonSocial; set => _razonSocial = value; }
        public string Cuit { get => _cuit; set => _cuit = value; }
        public string Telefono { get => _telefono; set => _telefono = value; }
        public string Email { get => _email; set => _email = value; }
        public string Direccion { get => _direccion; set => _direccion = value; }
        public bool Activo { get => _activo; set => _activo = value; }

        public Laboratorio() { }

        public Laboratorio(int idLaboratorio, string razonSocial, string cuit, string telefono, string email, string direccion, bool activo)
        {
            _idLaboratorio = idLaboratorio;
            _razonSocial = razonSocial;
            _cuit = cuit;
            _telefono = telefono;
            _email = email;
            _direccion = direccion;
            _activo = activo;
        }
    }
}