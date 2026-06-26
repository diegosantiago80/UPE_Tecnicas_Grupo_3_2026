using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Perfil
    {
        private int _idPerfil;
        private string _permisosAsignados = string.Empty;
        private string _descripcion = string.Empty;

        public int IdPerfil { get => _idPerfil; set => _idPerfil = value; }
        public string PermisosAsignados { get => _permisosAsignados; set => _permisosAsignados = value; }
        public string Descripcion { get => _descripcion; set => _descripcion = value; }

        public Perfil() { }

        public Perfil(int idPerfil, string permisosAsignados, string descripcion)
        {
            _idPerfil = idPerfil;
            _permisosAsignados = permisosAsignados;
            _descripcion = descripcion;
        }
    }
}