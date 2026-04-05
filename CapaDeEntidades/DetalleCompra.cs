using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class DetalleCompra
    {
        private int _idDetalleCompra;
        private int _idCompra;
        private int _idMedicamento;
        private int _cantidad;
        private decimal _precioUnitario;

        public int IdDetalleCompra { get => _idDetalleCompra; set => _idDetalleCompra = value; }
        public int IdCompra { get => _idCompra; set => _idCompra = value; }
        public int IdMedicamento { get => _idMedicamento; set => _idMedicamento = value; }
        public int Cantidad { get => _cantidad; set => _cantidad = value; }
        public decimal PrecioUnitario { get => _precioUnitario; set => _precioUnitario = value; }

        public decimal Subtotal { get => _cantidad * _precioUnitario; }

        public DetalleCompra() { }

        public DetalleCompra(int idDetalleCompra, int idCompra, int idMedicamento, int cantidad, decimal precioUnitario)
        {
            _idDetalleCompra = idDetalleCompra;
            _idCompra = idCompra;
            _idMedicamento = idMedicamento;
            _cantidad = cantidad;
            _precioUnitario = precioUnitario;
        }
    }
}