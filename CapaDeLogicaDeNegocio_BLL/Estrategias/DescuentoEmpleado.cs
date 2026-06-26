using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeLogicaDeNegocio_BLL.Estrategias
{
    public class DescuentoEmpleado : ICalculadorDescuento
    {
        public decimal CalcularTotal(decimal subtotal, bool aplicaDescuento)
        {
            return subtotal * 0.90m; // El empleado siempre tiene un 10% de descuento en todos los productos
        }
    }
}

