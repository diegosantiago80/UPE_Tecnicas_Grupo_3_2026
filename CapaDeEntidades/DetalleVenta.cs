using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class DetalleVenta
    {
        private int _idDetalleVenta;
        private int _idVenta;
        private int _idMedicamento;
        private int _cantidad;
        private decimal _precioUnitario;

        public int IdDetalleVenta { get => _idDetalleVenta; set => _idDetalleVenta = value; }
        public int IdVenta { get => _idVenta; set => _idVenta = value; }
        public int IdMedicamento { get => _idMedicamento; set => _idMedicamento = value; }
        public int Cantidad { get => _cantidad; set => _cantidad = value; }
        public decimal PrecioUnitario { get => _precioUnitario; set => _precioUnitario = value; }

        public decimal Subtotal
        {
            get => _cantidad * _precioUnitario;
        }

        public DetalleVenta() { }

        public DetalleVenta(int idDetalleVenta, int idVenta, int idMedicamento, int cantidad, decimal precioUnitario)
        {
            _idDetalleVenta = idDetalleVenta;
            _idVenta = idVenta;
            _idMedicamento = idMedicamento;
            _cantidad = cantidad;
            _precioUnitario = precioUnitario;
        }
    }
}