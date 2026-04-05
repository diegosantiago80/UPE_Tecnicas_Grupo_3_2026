using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    public class Compra
    {
        private int _idCompra;
        private DateTime _fecha;
        private int _idLaboratorio;
        private int _idUsuario;
        private decimal _total;
        private List<DetalleCompra> _detalle;

        public int IdCompra { get => _idCompra; set => _idCompra = value; }
        public DateTime Fecha { get => _fecha; set => _fecha = value; }
        public int IdLaboratorio { get => _idLaboratorio; set => _idLaboratorio = value; }
        public int IdUsuario { get => _idUsuario; set => _idUsuario = value; }
        public decimal Total { get => _total; set => _total = value; }
        public List<DetalleCompra> Detalle { get => _detalle; set => _detalle = value; }

        public Compra()
        {
            _detalle = new List<DetalleCompra>();
        }

        public Compra(int idCompra, DateTime fecha, int idLaboratorio, int idUsuario, decimal total, List<DetalleCompra> detalle)
        {
            _idCompra = idCompra;
            _fecha = fecha;
            _idLaboratorio = idLaboratorio;
            _idUsuario = idUsuario;
            _total = total;
            _detalle = detalle;
        }
    }
}