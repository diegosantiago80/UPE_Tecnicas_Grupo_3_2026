namespace CapaDeLogicaDeNegocio_BLL.Estrategias
{
    public class DescuentoObraSocial : ICalculadorDescuento
    {
        public decimal CalcularTotal(decimal subtotal, bool aplicaDescuento)
        {
            // Si el medicamento requiere receta (aplicaDescuento = true), hacemos el 40%
            // Si es venta libre, cobramos el subtotal entero
            return aplicaDescuento ? (subtotal * 0.60m) : subtotal;
        }
    }
}
