namespace CapaDeLogicaDeNegocio_BLL.Estrategias
{
    public class DescuentoObraSocial : ICalculadorDescuento
    {
        public decimal CalcularTotal(decimal subtotal)
        {
            return subtotal * 0.60m; // Aplica el 40% de descuento
        }
    }
}
