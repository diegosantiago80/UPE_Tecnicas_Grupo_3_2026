using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Medicamento
    {
        private int _idMedicamento;
        private string _nombre = string.Empty;
        private string _descripcion = string.Empty;
        private decimal _precioVenta;
        private decimal _precioCompra;
        private int _stockActual;
        private int _stockMinimo;
        private bool _requiereReceta;
        private int _idCategoria;
        private bool _activo;

        public int IdMedicamento { get => _idMedicamento; set => _idMedicamento = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }
        public string Descripcion { get => _descripcion; set => _descripcion = value; }
        public decimal PrecioVenta { get => _precioVenta; set => _precioVenta = value; }
        public decimal PrecioCompra { get => _precioCompra; set => _precioCompra = value; }
        public int StockActual { get => _stockActual; set => _stockActual = value; }
        public int StockMinimo { get => _stockMinimo; set => _stockMinimo = value; }
        public bool RequiereReceta { get => _requiereReceta; set => _requiereReceta = value; }
        public int IdCategoria { get => _idCategoria; set => _idCategoria = value; }
        public bool Activo { get => _activo; set => _activo = value; }

        public bool TieneStockCritico
        {
            get => _stockActual < _stockMinimo;
        }

        public Medicamento() { }

        public Medicamento(int idMedicamento, string nombre, string descripcion, decimal precioVenta, decimal precioCompra, int stockActual, int stockMinimo, bool requiereReceta, int idCategoria, bool activo)
        {
            _idMedicamento = idMedicamento;
            _nombre = nombre;
            _descripcion = descripcion;
            _precioVenta = precioVenta;
            _precioCompra = precioCompra;
            _stockActual = stockActual;
            _stockMinimo = stockMinimo;
            _requiereReceta = requiereReceta;
            _idCategoria = idCategoria;
            _activo = activo;
        }
    }
}