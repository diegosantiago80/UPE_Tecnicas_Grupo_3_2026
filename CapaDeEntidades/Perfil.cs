using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Perfil
    {
        private int _idPerfil;
        private string _descripcion = string.Empty;

        public int IdPerfil { get => _idPerfil; set => _idPerfil = value; }
        public string Descripcion { get => _descripcion; set => _descripcion = value; }

        public Perfil() { }

        public Perfil(int idPerfil, string descripcion)
        {
            _idPerfil = idPerfil;
            _descripcion = descripcion;
        }
    }
}