using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    // Capa de Entidades/ DTOs
    // Su propósito es definir los "moldes" o clases que representan las tablas de la base de datos.
    // Esta es la única capa que puede ser referenciada por todas las demás (Datos, Lógica y Presentación).
    public class Reporte
    {

        // Atributos Privados
        private string _nombreVendedor = string.Empty;
        private DateTime _fecha;
        private string _medicamento = string.Empty;
        private int _stockDisponible;
        private int _unidadesVendidas;
        private decimal _porcentajeVariacion;
        private int _idVenta;
        private decimal _monto;

        // Propiedades Públicas con Encapsulamiento
        public string NombreVendedor
        {
            get { return _nombreVendedor; }
            set { _nombreVendedor = value; }
        }

        public DateTime Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }

        public string Medicamento
        {
            get { return _medicamento; }
            set { _medicamento = value; }
        }

        // Esta propiedad mapea el nombre del medicamento según la BLL template
        public string NombreMedicamento
        {
            get { return _medicamento; }
            set { _medicamento = value; }
        }

        public int StockDisponible
        {
            get { return _stockDisponible; }
            set { _stockDisponible = value; }
        }

        public int UnidadesVendidas
        {
            get { return _unidadesVendidas; }
            set { _unidadesVendidas = value; }
        }

        public decimal PorcentajeVariacion
        {
            get { return _porcentajeVariacion; }
            set { _porcentajeVariacion = value; }
        }

        public int IdVenta
        {
            get { return _idVenta; }
            set { _idVenta = value; }
        }

        public decimal Monto
        {
            get { return _monto; }
            set { _monto = value; }
        }
    }
}
