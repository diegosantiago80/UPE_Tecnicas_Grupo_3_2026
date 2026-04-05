using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Categoria
    {
        private int _idCategoria;
        private string _nombre = string.Empty;
        private string _descripcion = string.Empty;

        public int IdCategoria { get => _idCategoria; set => _idCategoria = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }
        public string Descripcion { get => _descripcion; set => _descripcion = value; }

        public Categoria() { }

        public Categoria(int idCategoria, string nombre, string descripcion)
        {
            _idCategoria = idCategoria;
            _nombre = nombre;
            _descripcion = descripcion;
        }
    }
}