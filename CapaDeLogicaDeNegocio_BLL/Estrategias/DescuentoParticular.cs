namespace CapaDeLogicaDeNegocio_BLL.Estrategias
{
    public class DescuentoParticular : ICalculadorDescuento
    {
        public decimal CalcularTotal(decimal subtotal, bool aplicaDescuento)
        {
            return subtotal; // El particular siempre paga el 100%
        }
    }
}
