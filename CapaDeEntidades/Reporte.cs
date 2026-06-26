using System;

namespace CapaDeEntidades
{
    public class Reporte
    {
        private string _nombreVendedor = string.Empty;
        private DateTime _fecha;
        private string _medicamento = string.Empty;
        private int _stockDisponible;
        private int _unidadesVendidas;
        private decimal _porcentajeVariacion;
        private int _idVenta;
        private decimal _monto;

        public string NombreVendedor { get => _nombreVendedor; set => _nombreVendedor = value; }
        public DateTime Fecha { get => _fecha; set => _fecha = value; }
        public string Medicamento { get => _medicamento; set => _medicamento = value; }
        public int StockDisponible { get => _stockDisponible; set => _stockDisponible = value; }
        public int UnidadesVendidas { get => _unidadesVendidas; set => _unidadesVendidas = value; }
        public decimal PorcentajeVariacion { get => _porcentajeVariacion; set => _porcentajeVariacion = value; }
        public int IdVenta { get => _idVenta; set => _idVenta = value; }
        public decimal Monto { get => _monto; set => _monto = value; }
    }
}
