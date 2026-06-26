using CapaDeLogicaDeNegocio_BLL.Estrategias;
using Xunit;

namespace Pruebas_unitarias
{
    public class DescuentoTests
    {
        [Fact]
        public void DescuentoParticular_SiempreCobraElTotal()
        {
            // el particular paga el 100% sin importar si aplica descuento o no
            var estrategia = new DescuentoParticular();
            decimal subtotal = 1000m;

            decimal sinDescuento = estrategia.CalcularTotal(subtotal, aplicaDescuento: false);
            decimal conDescuento = estrategia.CalcularTotal(subtotal, aplicaDescuento: true);

            Assert.Equal(subtotal, sinDescuento);
            Assert.Equal(subtotal, conDescuento);
        }

        [Fact]
        public void DescuentoObraSocial_ConDescuento_Aplica40PorCiento()
        {
            // con obra social y medicamento con receta se aplica 40% de descuento (paga 60%)
            var estrategia = new DescuentoObraSocial();
            decimal subtotal = 1000m;

            decimal resultado = estrategia.CalcularTotal(subtotal, aplicaDescuento: true);

            Assert.Equal(600m, resultado);
        }
    }
}
