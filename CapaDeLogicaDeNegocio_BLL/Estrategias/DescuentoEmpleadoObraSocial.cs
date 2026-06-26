using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeLogicaDeNegocio_BLL.Estrategias
{
    public class DescuentoEmpleadoObraSocial : ICalculadorDescuento
    {
        public decimal CalcularTotal(decimal subtotal, bool aplicaDescuento)
        {
            decimal resultado = subtotal;

            // Primero aplica el descuento de obra social si el medicamento requiere receta
            if (aplicaDescuento)
            {
                resultado = resultado * 0.60m; // 40% de descuento obra social
            }

            // Luego aplica el 10% adicional por ser empleado
            resultado = resultado * 0.90m;

            return resultado;
        }
    }
}
