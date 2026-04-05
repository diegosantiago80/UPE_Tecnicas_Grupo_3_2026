using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Venta
    {
        private int _idVenta;
        private DateTime _fecha;
        private int _idCliente;
        private int _idUsuarioVendedor;
        private decimal _total;
        private List<DetalleVenta> _detalle;

        public int IdVenta { get => _idVenta; set => _idVenta = value; }
        public DateTime Fecha { get => _fecha; set => _fecha = value; }
        public int IdCliente { get => _idCliente; set => _idCliente = value; }
        public int IdUsuarioVendedor { get => _idUsuarioVendedor; set => _idUsuarioVendedor = value; }
        public decimal Total { get => _total; set => _total = value; }
        public List<DetalleVenta> Detalle { get => _detalle; set => _detalle = value; }

        public Venta()
        {
            _detalle = new List<DetalleVenta>();
        }

        public Venta(int idVenta, DateTime fecha, int idCliente, int idUsuarioVendedor, decimal total, List<DetalleVenta> detalle)
        {
            _idVenta = idVenta;
            _fecha = fecha;
            _idCliente = idCliente;
            _idUsuarioVendedor = idUsuarioVendedor;
            _total = total;
            _detalle = detalle;
        }
    }
}