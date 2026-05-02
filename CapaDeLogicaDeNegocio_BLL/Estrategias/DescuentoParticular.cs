namespace CapaDeLogicaDeNegocio_BLL.Estrategias
{
    public class DescuentoParticular : ICalculadorDescuento
    {
        public decimal CalcularTotal(decimal subtotal)
        {
            return subtotal; // Sin descuento
        }
    }
}
