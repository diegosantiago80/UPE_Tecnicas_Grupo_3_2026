using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CapaDeEntidades
{
    public class Auditoria
    {
        private DateTime _fechaHora;
        private string _usuario = string.Empty;
        private string _perfil = string.Empty;
        private string _modulo = string.Empty;
        private string _accion = string.Empty;

        public DateTime FechaHora { get => _fechaHora; set => _fechaHora = value; }
        public string Usuario { get => _usuario; set => _usuario = value; }
        public string Perfil { get => _perfil; set => _perfil = value; }
        public string Modulo { get => _modulo; set => _modulo = value; }
        public string Accion { get => _accion; set => _accion = value; }

        public Auditoria() { }

        public Auditoria(DateTime fechaHora, string usuario, string perfil, string modulo, string accion)
        {
            _fechaHora = fechaHora;
            _usuario = usuario;
            _perfil = perfil;
            _modulo = modulo;
            _accion = accion;
        }
    }
}